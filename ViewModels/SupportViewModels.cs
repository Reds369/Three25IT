using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Three25IT.Models;

namespace Three25IT.ViewModels;

public class SupportViewModel
{
    public List<SupportTicket> Tickets { get; set; } = new();
    public string? FilterStatus { get; set; }
    public string? FilterCategory { get; set; }
    public string? Search { get; set; }
}

public class NewTicketViewModel
{
    [Required(ErrorMessage = "Please choose a category.")]
    public string Category { get; set; } = TicketCategories.General;

    [Required(ErrorMessage = "Please choose a priority.")]
    public string Priority { get; set; } = TicketPriorities.Normal;

    [Required(ErrorMessage = "Please enter a subject.")]
    [StringLength(160, MinimumLength = 4, ErrorMessage = "Your subject must be at least 4 characters.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please describe the problem so we can help.")]
    [StringLength(4000, MinimumLength = 10, ErrorMessage = "Please provide at least 10 characters of detail.")]
    [Display(Name = "Describe the problem")]
    public string Description { get; set; } = string.Empty;

    public CustomerService? Service { get; set; }
}

public class TicketDetailViewModel
{
    public SupportTicket? Ticket { get; set; }
    public CustomerService? Service { get; set; }

    [Display(Name = "Add a reply")]
    [StringLength(2000, ErrorMessage = "Please keep your reply under 2000 characters.")]
    public string? Reply { get; set; }

    public bool CloseTicket { get; set; }
}

public class AccountViewModel
{
    public ApplicationUser? Customer { get; set; }
    public CustomerService? Service { get; set; }
    public Plan? Plan { get; set; }

    [Display(Name = "Full name")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [StringLength(30)]
    [Display(Name = "Contact number")]
    public string? Phone { get; set; }

    [StringLength(200)]
    [Display(Name = "Service address")]
    public string? StreetAddress { get; set; }

    [StringLength(100)]
    public string? Suburb { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(10)]
    [Display(Name = "Postal code")]
    public string? PostalCode { get; set; }

    [StringLength(50)]
    [Display(Name = "ID number")]
    public string? IdNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string AccountNumber { get; set; } = string.Empty;
}
