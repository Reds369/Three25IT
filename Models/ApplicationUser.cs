using Microsoft.AspNetCore.Identity;

namespace Three25IT.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? StreetAddress { get; set; }
    public string? Suburb { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Province { get; set; }
    public string? AccountNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? IdNumber { get; set; }

    public CustomerService? Service { get; set; }
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<SupportTicket> Tickets { get; set; } = new List<SupportTicket>();
    public ICollection<UsageRecord> Usage { get; set; } = new List<UsageRecord>();
}
