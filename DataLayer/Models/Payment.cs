using System.ComponentModel.DataAnnotations.Schema;

namespace APBD_PROJECT.DataLayer.Models;

public class Payment
{
    public long Id { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }

    public long ContractId { get; set; }
    public Contract Contract { get; set; } = null!;
}