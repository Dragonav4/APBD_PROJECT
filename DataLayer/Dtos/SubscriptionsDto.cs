namespace APBD_PROJECT.DataLayer.Dtos;

public class SubscriptionsDto
{
    public record SubscriptionCreateDto(
        long ClientId, 
        long SoftwareId, 
        string RenewalPeriod, 
        decimal Price);

    public record SubscriptionResponseDto(
        long Id,
        bool IsActive,
        DateTime StartDate,
        string RenewalPeriod,
        decimal Price);
}