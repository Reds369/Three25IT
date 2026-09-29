using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Three25IT.Models;

namespace Three25IT.ViewModels;

public class HomeViewModel
{
    public List<Plan> Plans { get; set; } = new();
    public CoverageCheckViewModel? Coverage { get; set; }
    public int HomesPassed { get; set; }
    public int SuburbsCovered { get; set; }
}

public class CoverageCheckViewModel
{
    [Required(ErrorMessage = "Please enter your street address, suburb or postal code.")]
    [StringLength(120, MinimumLength = 3, ErrorMessage = "Please enter at least 3 characters.")]
    [Display(Name = "Street address, suburb or postal code")]
    public string Address { get; set; } = string.Empty;

    public string? Normalised { get; set; }
    public bool Searched { get; set; }
    public string ResultType { get; set; } = "none";
    public string ResultTitle { get; set; } = string.Empty;
    public string ResultMessage { get; set; } = string.Empty;
    public List<ServiceArea> Matches { get; set; } = new();
    public ServiceArea? Primary { get; set; }
    public List<Plan> EligiblePlans { get; set; } = new();
    public bool ShowResults => Searched;
}

public class PricingViewModel
{
    public List<Plan> Plans { get; set; } = new();
    public string? HighlightSlug { get; set; }
}

public class ContactViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact number is required.")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Service address is required.")]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "I'm interested in")]
    public int? PlanId { get; set; }

    [StringLength(1000, ErrorMessage = "Please keep your message under 1000 characters.")]
    public string? Message { get; set; }

    public string? CoverageResult { get; set; }

    public List<SelectListItem> PlanOptions { get; set; } = new();
}

public class EnquirySuccessViewModel
{
    public string Reference { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool CoverageWasAvailable { get; set; }
}
