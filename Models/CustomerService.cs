namespace Three25IT.Models;

public class CustomerService
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public int PlanId { get; set; }
    public Plan? Plan { get; set; }

    public string ServiceStatus { get; set; } = ServiceStatuses.Active;
    public DateTime ActivatedOn { get; set; }
    public DateTime? TerminatedOn { get; set; }
    public DateTime ContractEndDate { get; set; }
    public DateTime NextBillingDate { get; set; }
    public decimal MonthlyPrice { get; set; }

    public string? FibreType { get; set; }
    public string? OntSerialNumber { get; set; }
    public string? PublicIpAddress { get; set; }
    public string? MacAddress { get; set; }
    public string? RouterModel { get; set; }
    public string? InstallationAddress { get; set; }
    public string? Notes { get; set; }
    public decimal OutstandingBalance { get; set; }
}

public static class ServiceStatuses
{
    public const string Active = "Active";
    public const string Pending = "Pending Installation";
    public const string Suspended = "Suspended";
    public const string Cancelled = "Cancelled";
}
