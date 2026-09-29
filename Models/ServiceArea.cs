namespace Three25IT.Models;

public class ServiceArea
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Suburb { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string? CoverageStatus { get; set; } = CoverageStatuses.Available;
    public int MaxDownloadMbps { get; set; }
    public int MaxUploadMbps { get; set; }
    public int? PlannedMonth { get; set; }
    public int? PlannedYear { get; set; }
    public DateTime? LastUpdated { get; set; }
    public string? ExchangeNode { get; set; }
    public int HomesPassed { get; set; }
    public decimal InstallFee { get; set; }
    public string? Notes { get; set; }
}

public static class CoverageStatuses
{
    public const string Available = "Available Now";
    public const string Planned = "Planned";
    public const string InProgress = "In Progress";
    public const string Waitlist = "Waitlist";
}
