namespace Three25IT.Models;

public class Payment
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public int? InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }

    public DateTime PaymentDate { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = PaymentStatuses.Successful;
    public string? CardLast4 { get; set; }
}

public static class PaymentStatuses
{
    public const string Successful = "Successful";
    public const string Pending = "Pending";
    public const string Failed = "Failed";
}
