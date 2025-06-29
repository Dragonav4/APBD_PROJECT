using System.ComponentModel.DataAnnotations;

namespace APBD_PROJECT.DataLayer.Models;

public class Client
{
    public long Id { get; set; }

    [Required, EmailAddress] public string Email { get; set; } = string.Empty;

    [Required, Phone] public string Phone { get; set; } = string.Empty;

    [Required] public string Address { get; set; } = string.Empty;

    public bool IsSoftDeleted { get; set; }

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    [StringLength(11)] public string? Pesel { get; set; }

    public string? CompanyName { get; set; }
    [StringLength(10)] public string? Krs { get; set; }
}