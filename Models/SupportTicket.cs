namespace Three25IT.Models;

public class SupportTicket
{
    public int Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Category { get; set; } = TicketCategories.General;
    public string Priority { get; set; } = TicketPriorities.Normal;
    public string TicketStatus { get; set; } = TicketStatuses.Open;

    public string? ApplicationUserId { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? AssignedTo { get; set; }

    public ICollection<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
}

public class TicketMessage
{
    public int Id { get; set; }
    public int SupportTicketId { get; set; }
    public SupportTicket? SupportTicket { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorUserId { get; set; }
    public bool IsFromSupport { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public static class TicketCategories
{
    public const string Technical = "Technical Fault";
    public const string Billing = "Billing";
    public const string Installation = "Installation";
    public const string Upgrading = "Upgrading";
    public const string General = "General";
}

public static class TicketPriorities
{
    public const string Low = "Low";
    public const string Normal = "Normal";
    public const string High = "High";
    public const string Urgent = "Urgent";
}

public static class TicketStatuses
{
    public const string Open = "Open";
    public const string AwaitingCustomer = "Awaiting Customer";
    public const string InProgress = "In Progress";
    public const string Resolved = "Resolved";
    public const string Closed = "Closed";
}
