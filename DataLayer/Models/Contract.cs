using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBD_PROJECT.DataLayer.Models;

public class Contract
{
    public long Id { get; set; }

    [Required] public DateTime StartDate { get; set; }

    [Required] public DateTime EndDate { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal Price { get; set; }

    [Range(0, 3)] public int SupportYears { get; set; }

    public bool IsSigned { get; set; }

    public long ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public long SoftwareVersionId { get; set; }
    public SoftwareVersion SoftwareVersion { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}