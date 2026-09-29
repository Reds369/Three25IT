using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Controllers;

[Authorize]
[Route("Service")]
public class ServiceController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ServiceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);
        var service = await _context.Services
            .AsNoTracking()
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);

        var plans = await _context.Plans
            .AsNoTracking()
            .Where(p => !p.IsBusiness)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        var usage = await _context.UsageRecords
            .AsNoTracking()
            .Where(u => u.ApplicationUserId == userId)
            .OrderByDescending(u => u.MonthStart)
            .FirstOrDefaultAsync(cancellationToken);

        var model = new MyServiceViewModel
        {
            CurrentUsage = usage,
            DataCapGb = 500m,
            AvailableUpgrades = plans
                .Where(p => service is null || p.DownloadMbps > (service.Plan?.DownloadMbps ?? 0))
                .ToList()
        };

        if (service is not null)
        {
            model.Customer = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            model.Service = service;
            model.Plan = service.Plan;
            model.DataUsedGb = usage?.DownloadGb ?? 0m;
            model.DataUsedPercent = model.DataCapGb > 0
                ? Math.Round(Math.Min(model.DataUsedGb / model.DataCapGb * 100m, 100m), 1)
                : 0m;

            var total = (service.ContractEndDate - service.ActivatedOn).TotalDays;
            var elapsed = (DateTime.UtcNow - service.ActivatedOn).TotalDays;
            model.ContractProgressPercent = total > 0
                ? Math.Round(Math.Clamp(elapsed / total * 100, 0, 100), 1)
                : 0;
        }

        return View(model);
    }

    [HttpPost("RequestUpgrade")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestUpgrade(int planId, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var plan = await _context.Plans
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == planId, cancellationToken);

        if (plan is null)
        {
            TempData["Error"] = "That plan could not be found.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null) return Challenge();

        var ticket = new SupportTicket
        {
            ApplicationUserId = userId,
            TicketNumber = GenerateTicketNumber(),
            Subject = $"Upgrade request: {plan.Name} ({plan.DownloadMbps} Mbps)",
            Category = TicketCategories.Upgrading,
            Priority = TicketPriorities.Normal,
            TicketStatus = TicketStatuses.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AssignedTo = "Sales & Provisioning"
        };

        ticket.Messages.Add(new TicketMessage
        {
            AuthorName = user.FullName ?? user.UserName ?? "Customer",
            AuthorUserId = userId,
            IsFromSupport = false,
            SentAt = DateTime.UtcNow,
            Body = $"Hi, I'd like to move to the {plan.Name} plan ({plan.DownloadMbps} Mbps down / {plan.UploadMbps} Mbps up) at R{plan.MonthlyPrice:0} per month. Please let me know the earliest date I can be switched over and whether there's any pro-rata charge."
        });

        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        TempData["Success"] = $"Your upgrade request to {plan.Name} has been logged. Our sales team will contact you shortly.";
        return RedirectToAction("Index", "Support");
    }

    private static string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1000, 9999)}";
    }
}
