using System.ComponentModel.DataAnnotations;

namespace Three25IT.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Keep me signed in")]
    public bool RememberMe { get; set; } = true;

    public string? ReturnUrl { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Please enter your full name.")]
    [StringLength(120, MinimumLength = 2)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a contact number.")]
    [StringLength(30)]
    [Display(Name = "Contact number")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a password.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Your password must be at least 8 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Please enter your service address.")]
    [StringLength(200)]
    [Display(Name = "Service address")]
    public string StreetAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your suburb or city.")]
    [StringLength(100)]
    public string Suburb { get; set; } = string.Empty;

    [StringLength(10)]
    [Display(Name = "Postal code")]
    public string? PostalCode { get; set; }

    public string? ReturnUrl { get; set; }
}
