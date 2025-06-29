using Microsoft.EntityFrameworkCore;
using System.Net;
using APBD_PROJECT.Data;
using APBD_PROJECT.DataLayer;
using APBD_PROJECT.DataLayer.Models;
using APBD_PROJECT.Exceptions;
using APBD_PROJECT.Interfaces;
using APBD_PROJECT.DataLayer.Dtos;

namespace APBD_PROJECT.Services;

public class SubscriptionService : ISubscriptionService
{
    private DatabaseContext _context;

    private async Task<decimal> GetLoyaltyPercentAsync()
    {
        var now = DateTime.Now;
        var loyalty = await _context.Discounts
            .FirstOrDefaultAsync(d => d.DiscountType == DiscountTarget.Loyalty &&
                                      d.StartDate <= now &&
                                      d.EndDate >= now);
        return loyalty?.Percentage ?? 0m;
    }

    private const int GraceDays = 0;

    public SubscriptionService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<SubscriptionsDto.SubscriptionResponseDto> CreateSubscriptionAsync(
        SubscriptionsDto.SubscriptionCreateDto dto)
    {
        var renewal = await ValidateRenewalPeriodAsync(dto);

        var (now, maxDiscount, discountPercent) = await CalculateDiscountAsync(dto);

        decimal finalPrice = dto.Price - dto.Price * (discountPercent / 100);

        var subscription = new Subscription
        {
            ClientId = dto.ClientId,
            SoftwareId = dto.SoftwareId,
            StartDate = now,
            RenewalPeriod = renewal,
            Price = dto.Price,
            IsActive = true
        };

        if (maxDiscount != null)
            subscription.Discounts.Add(maxDiscount);

        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();

        var payment = new SubscriptionPayment
        {
            SubscriptionId = subscription.Id,
            Amount = finalPrice,
            PaymentDate = now
        };

        await _context.SubscriptionPayments.AddAsync(payment);
        await _context.SaveChangesAsync();
        return ToDto(subscription);
    }


    public async Task AddRenewalPaymentAsync(long subscriptionId, decimal amount)
    {
        var subscription = await GetSubscription(subscriptionId);
        if (subscription.IsActive == false)
            throw new BadRequestException("Subscription is not active", HttpStatusCode.BadRequest);

        var (nextDue, dueEnd, windowEnd) = await CalculateBoundariesOfCurrentPeriod(subscriptionId, subscription);

        var now = DateTime.Now;
        if (now < nextDue || now > windowEnd)
            throw new BadRequestException("Renewal period expired", HttpStatusCode.BadRequest);

        decimal loyaltyPercent = await GetLoyaltyPercentAsync();
        decimal expectedAmount = subscription.Price * (1 - loyaltyPercent / 100m);
        if (expectedAmount != amount)
            throw new BadRequestException($"Subscription price invalid, {expectedAmount} PLN expected",
                HttpStatusCode.BadRequest);

        bool alreadyRenewedPaid =
            await _context.SubscriptionPayments.AnyAsync(p => p.PaymentDate >= nextDue && p.PaymentDate <= dueEnd);

        if (alreadyRenewedPaid)
            throw new BadRequestException("Subscription price already paid", HttpStatusCode.BadRequest);
        var payment = new SubscriptionPayment
        {
            SubscriptionId = subscriptionId,
            Amount = amount,
            PaymentDate = now
        };
        await _context.SubscriptionPayments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }

    public async Task CancelSubscriptionAsync(long subscriptionId)
    {
        var subscription = await GetSubscription(subscriptionId);
        if (!subscription.IsActive) return;
        var (nextDue, dueEnd, windowEnd) = await CalculateBoundariesOfCurrentPeriod(subscriptionId, subscription);
        bool paid = subscription.Payments
            .Any(p => p.PaymentDate >= nextDue && p.PaymentDate < dueEnd);

        if (!paid && DateTime.UtcNow > windowEnd)
        {
            subscription.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<SubscriptionsDto.SubscriptionResponseDto> GetSubscriptionByIdAsync(long id)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subscription == null)
            throw new BadRequestException("Subscription not found", HttpStatusCode.NotFound);

        return ToDto(subscription);
    }

    public async Task<List<SubscriptionsDto.SubscriptionResponseDto>> GetSubscriptionsForClientAsync(long clientId)
    {
        bool exists = await _context.Clients.AnyAsync(c => c.Id == clientId);
        if (!exists) throw new BadRequestException("Client not found", HttpStatusCode.NotFound);
        var list = await _context.Subscriptions
            .Where(s => s.ClientId == clientId)
            .Include(s => s.Payments)
            .ToListAsync();
        return list.Select(ToDto).ToList();
    }

    public async Task<DateTime> GetNextRenewalPeriodAsync(long subscriptionId)
    {
        var subscription = await GetSubscription(subscriptionId);
        int paidPeriods = subscription.Payments.Count;
        return subscription.StartDate.AddMonths(paidPeriods * PeriodInMonths(subscription.RenewalPeriod));
    }

    public async Task<decimal> GetCurrentSubscriptionRevenueAsync()
    {
        return await _context.SubscriptionPayments
            .Where(p => p.Subscription.IsActive)
            .SumAsync(p => p.Amount);
    }

    public async Task<decimal> GetPredictedSubscriptionRevenueAsync()
    {
        decimal current = await GetCurrentSubscriptionRevenueAsync();

        decimal loyaltyPercent = await GetLoyaltyPercentAsync();
        decimal nextPeriods = await _context.Subscriptions
            .Where(s => s.IsActive)
            .Select(s => s.Price * (1 - loyaltyPercent / 100m))
            .SumAsync();

        return current + nextPeriods;
    }

    private async Task<(DateTime nextDue, DateTime dueEnd, DateTime windowEnd)> CalculateBoundariesOfCurrentPeriod(
        long subscriptionId, Subscription subscription)
    {
        DateTime nextDue = await GetNextRenewalPeriodAsync(subscriptionId);
        DateTime dueEnd = nextDue.AddMonths(PeriodInMonths(subscription.RenewalPeriod));
        DateTime windowEnd = dueEnd.AddDays(GraceDays);
        return (nextDue, dueEnd, windowEnd);
    }

    private async Task<(DateTime now, Discount? maxDiscount, decimal discountPercent)> CalculateDiscountAsync(
        SubscriptionsDto.SubscriptionCreateDto dto)
    {
        var now = DateTime.Now;
        var maxDiscount = await _context.Discounts
            .Where(d => d.AppliesTo == DiscountTarget.Subscription && d.StartDate <= now && d.EndDate >= now)
            .OrderByDescending(d => d.Percentage)
            .FirstOrDefaultAsync();

        decimal discountPercent = (maxDiscount?.Percentage ?? 0);

        bool isLoyal = await _context.Contracts.AnyAsync(c => c.ClientId == dto.ClientId && c.IsSigned)
                       || await _context.Subscriptions.AnyAsync(s => s.ClientId == dto.ClientId);

        if (isLoyal)
            discountPercent += await GetLoyaltyPercentAsync();
        return (now, maxDiscount, discountPercent);
    }

    private async Task<RenewalPeriod> ValidateRenewalPeriodAsync(SubscriptionsDto.SubscriptionCreateDto dto)
    {
        bool clientExists = await _context.Clients
            .AnyAsync(c => c.Id == dto.ClientId);
        if (!clientExists)
            throw new KeyNotFoundException("Client not found");

        bool softwareExists = await _context.Software
            .AnyAsync(s => s.Id == dto.SoftwareId);
        if (!softwareExists)
            throw new KeyNotFoundException("Software not found");

        if (!Enum.TryParse(dto.RenewalPeriod, out RenewalPeriod renewal))
            throw new ArgumentException("Wrong renewal period");

        if (renewal < RenewalPeriod.Monthly || (int)renewal > (int)RenewalPeriod.Yearly * 2)
            throw new ArgumentException("Period should be between 1 month and 2 years");

        bool hasActive = await _context.Subscriptions.AnyAsync(s =>
            s.ClientId == dto.ClientId && s.SoftwareId == dto.SoftwareId && s.IsActive);

        if (hasActive)
            throw new InvalidOperationException("Client has already this subscription");
        return renewal;
    }

    private int PeriodInMonths(RenewalPeriod subscriptionRenewalPeriod) => (int)subscriptionRenewalPeriod;

    private async Task<Subscription> GetSubscription(long subscriptionId)
    {
        var subscription = await _context.Subscriptions.Include(s => s.Payments)
                               .FirstOrDefaultAsync(s => s.Id == subscriptionId)
                           ?? throw new BadRequestException("Subscription not found", HttpStatusCode.NotFound);
        return subscription;
    }

    private static SubscriptionsDto.SubscriptionResponseDto ToDto(Subscription s) =>
        new(s.Id, s.IsActive, s.StartDate, s.RenewalPeriod.ToString(), s.Price);
}