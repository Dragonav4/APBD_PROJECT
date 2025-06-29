namespace APBD_PROJECT.DataLayer.Dtos;

public class ContractPayments
{
    public record ContractCreateDto(
        long ClientId, 
        long SoftwareVersionId, 
        DateTime StartDate, 
        DateTime EndDate, 
        int SupportYears);
    public record ContractResponseDto(
        long Id, 
        decimal Price, 
        bool IsSigned, 
        DateTime StartDate, 
        DateTime EndDate);
}