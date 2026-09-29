namespace Three25IT.Models;

public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    public decimal Subtotal { get; set; }
    public decimal VAT { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public string InvoiceStatus { get; set; } = InvoiceStatuses.Draft;

    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public decimal Balance => Total - AmountPaid;
}

public class InvoiceLineItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
}

public static class InvoiceStatuses
{
    public const string Draft = "Draft";
    public const string Due = "Due";
    public const string Paid = "Paid";
    public const string Overdue = "Overdue";
}
