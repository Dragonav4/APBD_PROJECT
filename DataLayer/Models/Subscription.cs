using System.ComponentModel.DataAnnotations.Schema;

namespace APBD_PROJECT.DataLayer.Models;

public class Subscription
{
    public long Id { get; set; }
    public DateTime StartDate { get; set; }
    public RenewalPeriod RenewalPeriod { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }
    public bool IsActive { get; set; }

    public long ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public long SoftwareId { get; set; }
    public Software Software { get; set; } = null!;

    public ICollection<SubscriptionPayment> Payments { get; set; } = new List<SubscriptionPayment>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}

public class SubscriptionPayment
{
    public long Id { get; set; }
    public DateTime PaymentDate { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }

    public long SubscriptionId { get; set; }
    public Subscription Subscription { get; set; } = null!;
}