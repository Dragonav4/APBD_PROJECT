using APBD_PROJECT.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using APBD_PROJECT.Interfaces;

namespace APBD_PROJECT.Services;

public class RevenueService : IRevenueService
{
    private readonly DatabaseContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    private const decimal LoyalDiscountPercent = 5m;

    public RevenueService(DatabaseContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<decimal> GetActualRevenueAsync(
        DateTime fromInclusive,
        DateTime toExclusive)
    {
        if (fromInclusive >= toExclusive)
            throw new ArgumentException("'fromInclusive' must be earlier than 'toExclusive'.");

        var contractRev = await _context.Payments
            .Where(p => p.PaymentDate >= fromInclusive &&
                        p.PaymentDate < toExclusive)
            .SumAsync(p => p.Amount);

        var subsRev = await _context.SubscriptionPayments
            .Where(p => p.PaymentDate >= fromInclusive &&
                        p.PaymentDate < toExclusive)
            .SumAsync(p => p.Amount);

        return contractRev + subsRev;
    }

    public async Task<decimal> GetActualRevenueForProductAsync(
        long softwareId,
        DateTime fromInclusive,
        DateTime toExclusive)
    {
        if (fromInclusive >= toExclusive)
            throw new ArgumentException("'fromInclusive' must be earlier than 'toExclusive'.");

        var contractRev = await _context.Payments
            .Include(p => p.Contract)
            .ThenInclude(c => c.SoftwareVersion)
            .Where(p => p.Contract.SoftwareVersion.SoftwareId == softwareId &&
                        p.PaymentDate >= fromInclusive &&
                        p.PaymentDate < toExclusive)
            .SumAsync(p => p.Amount);

        var subsRev = await _context.SubscriptionPayments
            .Include(p => p.Subscription)
            .Where(p => p.Subscription.SoftwareId == softwareId &&
                        p.PaymentDate >= fromInclusive &&
                        p.PaymentDate < toExclusive)
            .SumAsync(p => p.Amount);

        return contractRev + subsRev;
    }

    public async Task<decimal> GetPredictedRevenueForProductAsync(long softwareId)
    {
        DateTime now = DateTime.UtcNow;

        var unsignedContracts = await _context.Contracts
            .Where(c => !c.IsSigned &&
                        c.SoftwareVersion.SoftwareId == softwareId &&
                        now >= c.StartDate && now <= c.EndDate)
            .SumAsync(c => c.Price);

        var nextSubscriptions = await _context.Subscriptions
            .Where(s => s.IsActive && s.SoftwareId == softwareId)
            .Select(s => s.Price * (1 - LoyalDiscountPercent / 100m))
            .SumAsync();

        return unsignedContracts + nextSubscriptions;
    }


    public async Task<decimal> GetPredictedRevenueAsync()
    {
        DateTime now = DateTime.UtcNow;

        var unsignedContracts = await _context.Contracts
            .Where(c => !c.IsSigned && now >= c.StartDate && now <= c.EndDate)
            .SumAsync(c => c.Price);
        var nextSubscription = await _context.Subscriptions
            .Where(s => s.IsActive)
            .Select(s => s.Price * (1 - LoyalDiscountPercent / 100m))
            .SumAsync();
        return unsignedContracts + nextSubscription;
    }

    public async Task<decimal> ConvertPlnToAsync(decimal amountPln, string targetCurrency)
    {
        if (string.Equals(targetCurrency, "PLN", StringComparison.OrdinalIgnoreCase))
            return amountPln;

        string url = $"https://api.nbp.pl/api/exchangerates/rates/A/{targetCurrency}/?format=json";

        using var client = _httpClientFactory.CreateClient();
        using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var json = await JsonDocument.ParseAsync(stream);

        decimal rate = json.RootElement
            .GetProperty("rates")[0]
            .GetProperty("mid")
            .GetDecimal();
        return amountPln / rate;
    }
}