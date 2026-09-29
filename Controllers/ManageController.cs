using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Controllers;

[Authorize]
[Route("Account/Manage")]
public class ManageController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ManageController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null) return Challenge();

        var service = await _context.Services
            .AsNoTracking()
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);

        return View(new AccountViewModel
        {
            Customer = user,
            Service = service,
            Plan = service?.Plan,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Phone = user.PhoneNumber,
            StreetAddress = user.StreetAddress,
            Suburb = user.Suburb,
            City = user.City,
            PostalCode = user.PostalCode,
            IdNumber = user.IdNumber,
            DateOfBirth = user.DateOfBirth,
            AccountNumber = user.AccountNumber ?? service?.AccountNumber ?? string.Empty
        });
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AccountViewModel model, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return Challenge();

        if (!ModelState.IsValid)
        {
            model.Customer = user;
            model.Service = await _context.Services
                .AsNoTracking().Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);
            model.Plan = model.Service?.Plan;
            model.AccountNumber = user.AccountNumber ?? string.Empty;
            return View(model);
        }

        var email = model.Email.Trim();
        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            var emailUser = await _userManager.FindByEmailAsync(email);
            if (emailUser is not null && emailUser.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.Email), "That email address is already linked to another account.");
                model.Customer = user;
                model.AccountNumber = user.AccountNumber ?? string.Empty;
                return View(model);
            }
        }

        user.FullName = model.FullName?.Trim();
        user.Email = email;
        user.NormalizedEmail = _userManager.NormalizeEmail(email);
        user.PhoneNumber = model.Phone?.Trim();
        user.StreetAddress = model.StreetAddress?.Trim();
        user.Suburb = model.Suburb?.Trim();
        user.City = model.City?.Trim();
        user.PostalCode = model.PostalCode?.Trim();
        user.IdNumber = model.IdNumber?.Trim();
        user.DateOfBirth = model.DateOfBirth;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            model.Customer = user;
            model.AccountNumber = user.AccountNumber ?? string.Empty;
            return View(model);
        }

        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);

        if (service is not null && !string.IsNullOrWhiteSpace(user.StreetAddress))
        {
            service.InstallationAddress = string.Join(", ", new[] { user.StreetAddress, user.Suburb, user.PostalCode }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _signInRefresh();
        TempData["Success"] = "Your account details have been updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("ChangePassword")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string? currentPassword, string? newPassword, string? confirmPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null) return Challenge();

        if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
        {
            TempData["Error"] = "Please enter your current password and a new password.";
            return RedirectToAction(nameof(Index));
        }

        if (newPassword.Length < 8)
        {
            TempData["Error"] = "Your new password must be at least 8 characters long.";
            return RedirectToAction(nameof(Index));
        }

        if (newPassword != confirmPassword)
        {
            TempData["Error"] = "The new password and confirmation password do not match.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        await _signInRefresh();
        TempData["Success"] = "Your password has been changed.";
        return RedirectToAction(nameof(Index));
    }

    private async Task _signInRefresh()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is not null)
        {
            await _signInManager.RefreshSignInAsync(user);
        }
    }
}
