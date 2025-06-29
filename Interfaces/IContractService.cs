using APBD_PROJECT.DataLayer.Dtos;

namespace APBD_PROJECT.Interfaces;

public interface IContractService
{
    Task<ContractPayments.ContractResponseDto> CreateContractAsync(ContractPayments.ContractCreateDto dto);
    Task<List<ContractPayments.ContractResponseDto>> GetContractsForClientAsync(long id);
    Task AddPaymentAsync(long contractId, decimal amount);

    Task<bool> IsRevenueRecognized(long contractId);
}