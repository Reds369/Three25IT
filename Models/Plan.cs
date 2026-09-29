namespace Three25IT.Models;

public class Plan
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DownloadMbps { get; set; }
    public int UploadMbps { get; set; }
    public decimal MonthlyPrice { get; set; }
    public string? DataCap { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? IncludedFeatures { get; set; }
    public bool IsUncapped { get; set; }
    public bool IsBusiness { get; set; }
    public int ContractMonths { get; set; }
    public bool IsFeatured { get; set; }
    public int SortOrder { get; set; }

    public ICollection<CustomerService> Services { get; set; } = new List<CustomerService>();
}
