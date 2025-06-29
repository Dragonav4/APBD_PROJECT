using System.ComponentModel.DataAnnotations;

namespace APBD_PROJECT.DataLayer.Models;

public class Software
{
    public long Id { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;

    public ICollection<SoftwareVersion> Versions { get; set; } = new List<SoftwareVersion>();
}

public class SoftwareVersion
{
    public long Id { get; set; }
    [Required] public string Version { get; set; } = string.Empty;

    public decimal YearlyPrice { get; set; }
    public long SoftwareId { get; set; }
    public Software Software { get; set; } = null!;
}