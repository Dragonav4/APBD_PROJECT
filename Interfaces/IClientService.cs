using APBD_PROJECT.DataLayer.Dtos;

namespace APBD_PROJECT.Interfaces;

public interface IClientService
{
    Task<ClientDto> AddClientAsync(ClientDto dto);
    Task<ClientDto> UpdateClientAsync(long id, ClientDto dto);
    Task<ClientDto> GetClientAsync(long id);
    Task DeleteClientAsync(long id);
}