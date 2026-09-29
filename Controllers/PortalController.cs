using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Controllers;

[Authorize]
[Route("Portal")]
public class PortalController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PortalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User), cancellationToken);

        if (user is null) return Challenge();

        var service = await _context.Services
            .AsNoTracking()
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id, cancellationToken);

        var invoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.ApplicationUserId == user.Id)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);

        var openTickets = await _context.SupportTickets
            .AsNoTracking()
            .Where(t => t.ApplicationUserId == user.Id
                && t.TicketStatus != TicketStatuses.Closed
                && t.TicketStatus != TicketStatuses.Resolved)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync(cancellationToken);

        var usage = await _context.UsageRecords
            .AsNoTracking()
            .Where(u => u.ApplicationUserId == user.Id)
            .OrderByDescending(u => u.MonthStart)
            .Take(2)
            .ToListAsync(cancellationToken);

        var model = new DashboardViewModel
        {
            Customer = user,
            Service = service,
            Plan = service?.Plan,
            RecentInvoices = invoices.Take(4).ToList(),
            LatestInvoice = invoices.FirstOrDefault(),
            OutstandingInvoice = invoices.FirstOrDefault(i => i.Balance > 0),
            OpenTickets = openTickets,
            CurrentUsage = usage.FirstOrDefault(),
            PreviousUsage = usage.Skip(1).FirstOrDefault(),
            OpenTicketCount = openTickets.Count
        };

        model.OutstandingBalance = service?.OutstandingBalance ?? invoices.Sum(i => i.Balance);

        return View(model);
    }
}
