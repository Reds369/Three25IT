namespace Three25IT.Models;

public class Enquiry
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int? PlanId { get; set; }
    public Plan? Plan { get; set; }
    public string? CoverageResult { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
    public string EnquiryStatus { get; set; } = "New";
}
