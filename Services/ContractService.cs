using System.Net;
using APBD_PROJECT.Data;
using APBD_PROJECT.DataLayer;
using APBD_PROJECT.DataLayer.Dtos;
using APBD_PROJECT.DataLayer.Models;
using APBD_PROJECT.Exceptions;
using APBD_PROJECT.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APBD_PROJECT.Services;

public class ContractService : IContractService
{
    private readonly DatabaseContext _context;

    public ContractService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<ContractPayments.ContractResponseDto> CreateContractAsync(ContractPayments.ContractCreateDto dto)
    {
        var client = await _context
            .Clients
            .Include(c=>c.Contracts)
            .Include(s => s.Subscriptions)
            .FirstOrDefaultAsync(c=> c.Id == dto.ClientId) 
                     ?? throw new BadRequestException("Client not found",HttpStatusCode.NotFound);
        
        var actualDays = (dto.EndDate-dto.StartDate).TotalDays;
        if (actualDays <= 3 || actualDays >= 30) throw new BadRequestException("The contract term must be from 3 to 30 days",HttpStatusCode.BadRequest);
        bool hasActiveSubscription = client.Subscriptions
            .Any(s => s.IsActive && s.StartDate <= DateTime.Now);

        bool hasActiveContract = client.Contracts.Any(
            c => c.IsSigned && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now);
        if (hasActiveSubscription || hasActiveContract)
            throw new BadRequestException("Client already has contract or subscription", HttpStatusCode.BadRequest);

        var version = await _context.SoftwareVersions
                          .Include(v => v.Software)
                          .FirstOrDefaultAsync(v => v.Id == dto.SoftwareVersionId)
                      ?? throw new BadRequestException("Software version not found", HttpStatusCode.NotFound);

        var basePriceSw = version.YearlyPrice; 
        var supportPrice = dto.SupportYears * 1000m;
        var momentAtCreatingContract = DateTime.Now;
        var filteredDiscounts = await _context.Discounts
            .Where(d => d.AppliesTo == DiscountTarget.Upfront
                        && d.StartDate <= momentAtCreatingContract
                        && d.EndDate >= momentAtCreatingContract)
            .ToListAsync();

        var maxDiscount = filteredDiscounts
            .OrderByDescending(d => d.Percentage)
            .FirstOrDefault();

        var discountPercent = maxDiscount != null ? maxDiscount.Percentage : 0;
        var isLoyal = await _context.Contracts.AnyAsync(c => c.ClientId == client.Id && c.IsSigned) ||
                      await _context.Subscriptions.AnyAsync(s => s.ClientId == client.Id);

        if (isLoyal) discountPercent += 5;
        var totalPrice = basePriceSw + supportPrice;
        totalPrice -= totalPrice * (discountPercent / 100);

        var contract = new Contract
        {
            ClientId = client.Id,
            SoftwareVersionId = version.Id,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Price = totalPrice,
            SupportYears = dto.SupportYears,
            IsSigned = false
        };

        if (maxDiscount != null)
            contract.Discounts.Add(maxDiscount);

        await _context.Contracts.AddAsync(contract);
        await _context.SaveChangesAsync();
        return new ContractPayments.ContractResponseDto(
            contract.Id,
            contract.Price,
            contract.IsSigned,
            contract.StartDate,
            contract.EndDate
        );
    }

    public async Task<List<ContractPayments.ContractResponseDto>> GetContractsForClientAsync(long id)
    {
        bool clientExists = await _context.Clients.AnyAsync(c => c.Id == id);
        if (!clientExists)
            throw new BadRequestException("Client not found", HttpStatusCode.NotFound);
        var list = await _context.Contracts
            .Where(c => c.ClientId == id)
            .ToListAsync();
        return list.Select(c => new ContractPayments.ContractResponseDto(
            c.Id,
            c.Price,
            c.IsSigned,
            c.StartDate,
            c.EndDate
        )).ToList();
    }

    public async Task AddPaymentAsync(long contractId, decimal amount)
    {
        bool contractExists = await _context.Contracts.AnyAsync(c => c.Id == contractId);
        if (!contractExists) throw new BadRequestException("Contract not found", HttpStatusCode.NotFound);
        if (amount <= 0) throw new BadRequestException("Amount must be greater than 0", HttpStatusCode.BadRequest);
        var contract = await _context.Contracts.Include(contract => contract.Payments)
                           .FirstOrDefaultAsync(c => c.Id == contractId)
                       ?? throw new BadRequestException("Contract not found", HttpStatusCode.NotFound);
        if (contract.EndDate < DateTime.Now)
            throw new BadRequestException("Contract end date", HttpStatusCode.BadRequest);
        decimal totalPrice = contract.Payments.Sum(c => c.Amount);
        if (totalPrice + amount > contract.Price)
            throw new BadRequestException("The amount exceeded the contract price", HttpStatusCode.BadRequest);
        var payment = new Payment
        {
            Amount = amount,
            PaymentDate = DateTime.Now,
            ContractId = contractId
        };
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();

        totalPrice += amount;
        if (totalPrice == contract.Price)
        {
            contract.IsSigned = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsRevenueRecognized(long contractId)
    {
        var contract = await _context.Contracts.Include(contract => contract.Payments)
            .FirstOrDefaultAsync(c => c.Id == contractId);
        if (contract == null) throw new BadRequestException("Contract not found", HttpStatusCode.NotFound);
        var totalPaid = contract.Payments.Sum(c => c.Amount);
        return totalPaid >= contract.Price && contract.IsSigned;
    }
}