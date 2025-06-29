using System.ComponentModel.DataAnnotations;

namespace APBD_PROJECT.DataLayer.Models;

public class Discount
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public DiscountTarget DiscountType { get; set; }
    public long? SoftwareId { get; set; }
    public Software? Software { get; set; }

    [Range(0, 100)] public decimal Percentage { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public DiscountTarget AppliesTo { get; set; }
}