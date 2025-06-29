namespace APBD_PROJECT.Interfaces;

public interface IRevenueService
{
    Task<decimal> GetActualRevenueAsync(
        DateTime fromInclusive,
        DateTime toExclusive);

    Task<decimal> GetActualRevenueForProductAsync(
        long softwareId,
        DateTime fromInclusive,
        DateTime toExclusive);

    Task<decimal> GetPredictedRevenueForProductAsync(long softwareId);

    Task<decimal> GetPredictedRevenueAsync();

    public Task<decimal> ConvertPlnToAsync(decimal amountPln, string targetCurrency);


}