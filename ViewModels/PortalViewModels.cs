using System.ComponentModel.DataAnnotations;
using Three25IT.Models;

namespace Three25IT.ViewModels;

public class DashboardViewModel
{
    public ApplicationUser? Customer { get; set; }
    public CustomerService? Service { get; set; }
    public Plan? Plan { get; set; }
    public Invoice? LatestInvoice { get; set; }
    public Invoice? OutstandingInvoice { get; set; }
    public List<Invoice> RecentInvoices { get; set; } = new();
    public List<SupportTicket> OpenTickets { get; set; } = new();
    public UsageRecord? CurrentUsage { get; set; }
    public UsageRecord? PreviousUsage { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int OpenTicketCount { get; set; }
}

public class MyServiceViewModel
{
    public ApplicationUser? Customer { get; set; }
    public CustomerService? Service { get; set; }
    public Plan? Plan { get; set; }
    public List<Plan> AvailableUpgrades { get; set; } = new();
    public UsageRecord? CurrentUsage { get; set; }
    public decimal DataUsedGb { get; set; }
    public decimal DataCapGb { get; set; } = 500m;
    public decimal DataUsedPercent { get; set; }
    public double ContractProgressPercent { get; set; }
}

public class BillingViewModel
{
    public ApplicationUser? Customer { get; set; }
    public List<Invoice> Invoices { get; set; } = new();
    public List<Payment> Payments { get; set; } = new();
    public decimal OutstandingBalance { get; set; }
    public decimal TotalBilledYearToDate { get; set; }
    public decimal TotalPaidYearToDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
}

public class InvoiceDetailViewModel
{
    public Invoice? Invoice { get; set; }
    public ApplicationUser? Customer { get; set; }
    public CustomerService? Service { get; set; }
    public Plan? Plan { get; set; }
}

public class PaymentViewModel
{
    [Required(ErrorMessage = "Please select an invoice to pay.")]
    public int InvoiceId { get; set; }

    [Required(ErrorMessage = "Please select a payment method.")]
    public string PaymentMethod { get; set; } = "Card";

    public List<Invoice> PayableInvoices { get; set; } = new();
}

public class UsageViewModel
{
    public CustomerService? Service { get; set; }
    public Plan? Plan { get; set; }
    public List<UsageRecord> Records { get; set; } = new();
    public decimal TotalDownloadGb { get; set; }
    public decimal TotalUploadGb { get; set; }
    public decimal AvgDownloadGb { get; set; }
    public decimal PeakDownloadMbps { get; set; }
}
