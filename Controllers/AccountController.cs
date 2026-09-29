using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Controllers;

[Route("Account")]
public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToPortal();

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("Login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            ModelState.AddModelError(string.Empty, "We could not sign you in with those details. Please check your email and password, or create an account.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return RedirectToPortal(model.ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Your account is temporarily locked after too many failed attempts. Please try again in a few minutes.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "We could not sign you in with those details. Please check your email and password, or create an account.");
        return View(model);
    }

    [HttpGet("Register")]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToPortal();

        ViewData["ReturnUrl"] = returnUrl;
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("Register")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = model.Email.Trim();
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email address already exists. Please sign in instead.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            FullName = model.FullName?.Trim(),
            PhoneNumber = model.Phone?.Trim(),
            StreetAddress = model.StreetAddress?.Trim(),
            Suburb = model.Suburb?.Trim(),
            PostalCode = model.PostalCode?.Trim()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        // New enquiries get a stub service record so the portal has something to show.
        var plan = await _context.Plans
            .AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .FirstOrDefaultAsync();

        if (plan is not null)
        {
            var suburb = model.Suburb?.Trim();
            var area = await _context.ServiceAreas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Suburb == suburb);

            user.AccountNumber = GenerateAccountNumber();
            await _context.Services.AddAsync(new CustomerService
            {
                ApplicationUserId = user.Id,
                PlanId = plan.Id,
                AccountNumber = user.AccountNumber,
                ServiceStatus = ServiceStatuses.Pending,
                ActivatedOn = DateTime.UtcNow,
                ContractEndDate = DateTime.UtcNow.AddMonths(plan.ContractMonths),
                NextBillingDate = DateTime.UtcNow.AddDays(7),
                MonthlyPrice = plan.MonthlyPrice,
                FibreType = area is not null ? $"Single-mode GPON via {area.ExchangeNode}" : "Single-mode GPON",
                InstallationAddress = string.Join(", ", new[] { model.StreetAddress, model.Suburb, model.PostalCode }
                    .Where(s => !string.IsNullOrWhiteSpace(s)))
            });

            await _context.SaveChangesAsync();
        }
        else
        {
            user.AccountNumber = GenerateAccountNumber();
            await _userManager.UpdateAsync(user);
        }

        await _signInManager.SignInAsync(user, isPersistent: true);
        TempData["Success"] = "Welcome to Three25IT! Your customer account is ready.";

        return RedirectToPortal(model.ReturnUrl);
    }

    [HttpPost("Logout")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login), "Account");
    }

    [HttpGet("Logout")]
    [Authorize]
    public IActionResult LogoutGet() => RedirectToAction(nameof(Login), "Account");

    [HttpGet("AccessDenied")]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToPortal(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Portal");
    }

    private static string GenerateAccountNumber()
    {
        return $"T3-{DateTime.UtcNow.Year}-{Random.Shared.Next(100000, 999999)}";
    }
}
