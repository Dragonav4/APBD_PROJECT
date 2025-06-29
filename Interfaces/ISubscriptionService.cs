using APBD_PROJECT.DataLayer.Dtos;

namespace APBD_PROJECT.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionsDto.SubscriptionResponseDto> CreateSubscriptionAsync(SubscriptionsDto.SubscriptionCreateDto dto);
    Task AddRenewalPaymentAsync(long subscriptionId, decimal amount);

    Task CancelSubscriptionAsync(long subscriptionId);

    Task<List<SubscriptionsDto.SubscriptionResponseDto>> GetSubscriptionsForClientAsync(long clientId);

    Task<DateTime> GetNextRenewalPeriodAsync(long subscriptionId);

    Task<decimal> GetCurrentSubscriptionRevenueAsync();

    Task<decimal> GetPredictedSubscriptionRevenueAsync();

    Task<SubscriptionsDto.SubscriptionResponseDto> GetSubscriptionByIdAsync(long id);
}