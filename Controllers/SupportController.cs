using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Controllers;

[Authorize]
[Route("Support")]
public class SupportController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SupportController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? status, string? category, string? search, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var query = _context.SupportTickets
            .AsNoTracking()
            .Where(t => t.ApplicationUserId == userId);

        if (!string.IsNullOrWhiteSpace(status) && status != "All")
        {
            query = query.Where(t => t.TicketStatus == status);
        }

        if (!string.IsNullOrWhiteSpace(category) && category != "All")
        {
            query = query.Where(t => t.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.Subject.Contains(search) || t.TicketNumber.Contains(search));
        }

        var model = new SupportViewModel
        {
            FilterStatus = status,
            FilterCategory = category,
            Search = search,
            Tickets = await query
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync(cancellationToken)
        };

        return View(model);
    }

    [HttpGet("New")]
    public async Task<IActionResult> New(CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var model = new NewTicketViewModel
        {
            Service = await _context.Services
                .AsNoTracking()
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken)
        };

        return View(model);
    }

    [HttpPost("New")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> New(NewTicketViewModel model, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (!ModelState.IsValid)
        {
            model.Service = await _context.Services
                .AsNoTracking()
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);
            return View(model);
        }

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        var ticket = new SupportTicket
        {
            ApplicationUserId = userId,
            TicketNumber = GenerateTicketNumber(),
            Subject = model.Subject.Trim(),
            Category = model.Category,
            Priority = model.Priority,
            TicketStatus = TicketStatuses.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ticket.Messages.Add(new TicketMessage
        {
            AuthorName = user?.FullName ?? user?.UserName ?? "Customer",
            AuthorUserId = userId,
            IsFromSupport = false,
            SentAt = DateTime.UtcNow,
            Body = model.Description.Trim()
        });

        _context.SupportTickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);

        TempData["Success"] = $"Ticket {ticket.TicketNumber} has been created. Our team will respond shortly.";
        return RedirectToAction(nameof(Detail), new { id = ticket.Id });
    }

    [HttpGet("Detail/{id:int}")]
    public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var ticket = await _context.SupportTickets
            .AsNoTracking()
            .Include(t => t.Messages.OrderBy(m => m.SentAt))
            .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == userId, cancellationToken);

        if (ticket is null) return NotFound();

        var model = new TicketDetailViewModel
        {
            Ticket = ticket,
            Service = await _context.Services
                .AsNoTracking()
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken)
        };

        return View(model);
    }

    [HttpPost("Reply/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, TicketDetailViewModel model, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var ticket = await _context.SupportTickets
            .Include(t => t.Messages)
            .FirstOrDefaultAsync(t => t.Id == id && t.ApplicationUserId == userId, cancellationToken);

        if (ticket is null) return NotFound();

        if (model.CloseTicket)
        {
            ticket.TicketStatus = TicketStatuses.Closed;
            ticket.ClosedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = $"Ticket {ticket.TicketNumber} has been closed. You can open a new ticket any time.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        if (string.IsNullOrWhiteSpace(model.Reply))
        {
            ModelState.AddModelError(nameof(model.Reply), "Please type a reply before sending.");
        }
        else
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            ticket.Messages.Add(new TicketMessage
            {
                AuthorName = user?.FullName ?? user?.UserName ?? "Customer",
                AuthorUserId = userId,
                IsFromSupport = false,
                SentAt = DateTime.UtcNow,
                Body = model.Reply.Trim()
            });

            if (ticket.TicketStatus == TicketStatuses.AwaitingCustomer)
            {
                ticket.TicketStatus = TicketStatuses.Open;
            }

            ticket.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            TempData["Success"] = "Your reply has been sent.";
        }

        return RedirectToAction(nameof(Detail), new { id });
    }

    private static string GenerateTicketNumber()
    {
        return $"TKT-{DateTime.UtcNow:yyyy}-{Random.Shared.Next(1000, 9999)}";
    }
}
