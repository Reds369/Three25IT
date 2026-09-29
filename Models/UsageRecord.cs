namespace Three25IT.Models;

public class UsageRecord
{
    public int Id { get; set; }
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public string Month { get; set; } = string.Empty;
    public DateTime MonthStart { get; set; }
    public decimal DownloadGb { get; set; }
    public decimal UploadGb { get; set; }
    public decimal PeakDownloadMbps { get; set; }
    public decimal PeakUploadMbps { get; set; }
    public decimal AverageLatencyMs { get; set; }
}
