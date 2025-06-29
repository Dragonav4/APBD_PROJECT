using System.Net;
using Microsoft.EntityFrameworkCore;
using APBD_PROJECT.Data;
using APBD_PROJECT.DataLayer.Models;
using APBD_PROJECT.DataLayer.Dtos;
using APBD_PROJECT.Exceptions;
using APBD_PROJECT.Interfaces;

namespace APBD_PROJECT.Services;

public class ClientService : IClientService
{
    private readonly DatabaseContext _context;

    public ClientService(DatabaseContext context)
    {
        _context = context;
    }

    private static void ValidateCreate(ClientDto dto)
    {
        var hasPesel = !string.IsNullOrWhiteSpace(dto.Person?.Pesel);
        var hasKrs = !string.IsNullOrWhiteSpace(dto.Company?.Krs);

        if (hasPesel == hasKrs)
            throw new BadRequestException(
                "Either Pesel or Krs must be provided (but not both).",
                HttpStatusCode.BadRequest);
    }

    private static void ValidateUpdate(ClientDto dto)
    {
        var hasPesel = !string.IsNullOrWhiteSpace(dto.Person?.Pesel);
        var hasKrs = !string.IsNullOrWhiteSpace(dto.Company?.Krs);

        if (hasPesel && hasKrs)
            throw new BadRequestException(
                "Client cannot have both Pesel and Krs.",
                HttpStatusCode.BadRequest);
    }

    public async Task<ClientDto> AddClientAsync(ClientDto dto)
    {
        ValidateCreate(dto);

        if (!string.IsNullOrWhiteSpace(dto.Person?.Pesel) &&
            await _context.Clients.AnyAsync(c => c.Pesel == dto.Person.Pesel))
            throw new BadRequestException($"{dto.Person.Pesel} already exists.", HttpStatusCode.Conflict);

        if (!string.IsNullOrWhiteSpace(dto.Company?.Krs) &&
            await _context.Clients.AnyAsync(c => c.Krs == dto.Company.Krs))
            throw new BadRequestException($"{dto.Company.Krs} already exists.", HttpStatusCode.Conflict);

        var client = ToDbModel(new Client(), dto);

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        return ToResponse(client);
    }

    public async Task<ClientDto> UpdateClientAsync(long id, ClientDto dto)
    {
        ValidateUpdate(dto);

        var client = await _context.Clients.FindAsync(id)
                     ?? throw new BadRequestException($"Client {id} not found.", HttpStatusCode.NotFound);

        if (!string.IsNullOrWhiteSpace(dto.Person?.Pesel) && client.Pesel != dto.Person.Pesel)
            throw new BadRequestException($"Can't change PESEL from {client.Pesel} to {dto.Person.Pesel}.",
                HttpStatusCode.UnprocessableEntity);

        if (!string.IsNullOrWhiteSpace(dto.Company?.Krs) && client.Krs != dto.Company.Krs)
            throw new BadRequestException($"Can't change KRS from {client.Krs} to {dto.Company.Krs}.",
                HttpStatusCode.UnprocessableEntity);

        ToDbModel(client, dto);
        await _context.SaveChangesAsync();

        return ToResponse(client);
    }

    public async Task<ClientDto> GetClientAsync(long id)
    {
        var client = await _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id)
                     ?? throw new BadRequestException($"Client {id} not found.", HttpStatusCode.NotFound);

        return ToResponse(client);
    }

    public async Task DeleteClientAsync(long id)
    {
        var client = await _context.Clients.FindAsync(id)
                     ?? throw new BadRequestException($"Client {id} not found.", HttpStatusCode.NotFound);

        client.IsSoftDeleted = true;
        await _context.SaveChangesAsync();
    }

    private static Client ToDbModel(Client dbModel, ClientDto dto)
    {
        dbModel.Id = dto.Id;
        dbModel.FirstName = dto.Person?.FirstName;
        dbModel.LastName = dto.Person?.LastName;
        dbModel.CompanyName = dto.Company?.CompanyName;
        dbModel.Email = dto.Person?.Email ?? dto.Company?.Email;
        dbModel.Phone = dto.Person?.Phone ?? dto.Company?.Phone;
        dbModel.Address = dto.Person?.Address ?? dto.Company?.Address;
        dbModel.Pesel = dto.Person?.Pesel;
        dbModel.Krs = dto.Company?.Krs;

        return dbModel;
    }

    private static ClientDto ToResponse(Client c)
    {
        if (c.Pesel != null)
            return new ClientDto
            {
                Id = c.Id,
                Person = new PersonalClientDto
                {
                    Pesel = c.Pesel,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Address = c.Address,
                    Email = c.Email,
                    Phone = c.Phone,
                }
            };
        return new ClientDto
        {
            Id = c.Id,
            Company = new CompanyClientDto
            {
                Krs = c.Krs,
                CompanyName = c.CompanyName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
            }
        };
    }
}