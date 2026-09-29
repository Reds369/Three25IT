using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Controllers;

[Authorize]
[Route("Billing")]
public class BillingController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public BillingController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var invoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.ApplicationUserId == userId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);

        var payments = await _context.Payments
            .AsNoTracking()
            .Where(p => p.ApplicationUserId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .Take(12)
            .ToListAsync(cancellationToken);

        var service = await _context.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);

        var yearStart = new DateTime(DateTime.UtcNow.Year, 1, 1);
        var ytd = invoices.Where(i => i.IssueDate >= yearStart).ToList();

        var model = new BillingViewModel
        {
            Customer = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken),
            Invoices = invoices,
            Payments = payments,
            OutstandingBalance = service?.OutstandingBalance ?? invoices.Sum(i => i.Balance),
            TotalBilledYearToDate = ytd.Sum(i => i.Total),
            TotalPaidYearToDate = ytd.Sum(i => i.AmountPaid),
            NextBillingDate = service?.NextBillingDate
        };

        return View(model);
    }

    [HttpGet("Invoice/{id:int}")]
    public async Task<IActionResult> Invoice(int id, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var invoice = await _context.Invoices
            .AsNoTracking()
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.Id == id && i.ApplicationUserId == userId, cancellationToken);

        if (invoice is null) return NotFound();

        var service = await _context.Services
            .AsNoTracking()
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);

        return View(new InvoiceDetailViewModel
        {
            Invoice = invoice,
            Customer = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, cancellationToken),
            Service = service,
            Plan = service?.Plan
        });
    }

    [HttpGet("Pay")]
    public async Task<IActionResult> Pay(int? invoiceId, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var model = await BuildPaymentModelAsync(invoiceId, cancellationToken);

        var preselected = model.PayableInvoices.FirstOrDefault(i => i.Id == invoiceId);
        if (preselected is not null)
        {
            model.InvoiceId = preselected.Id;
        }

        return View(model);
    }

    [HttpPost("Pay")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(PaymentViewModel model, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        if (!ModelState.IsValid)
        {
            var rebuilt = await BuildPaymentModelAsync(model.InvoiceId, cancellationToken);
            rebuilt.InvoiceId = model.InvoiceId;
            rebuilt.PaymentMethod = model.PaymentMethod;
            return View(rebuilt);
        }

        var invoice = await _context.Invoices
            .Include(i => i.LineItems)
            .FirstOrDefaultAsync(i => i.Id == model.InvoiceId && i.ApplicationUserId == userId, cancellationToken);

        if (invoice is null || invoice.Balance <= 0)
        {
            TempData["Error"] = "That invoice cannot be paid.";
            return RedirectToAction(nameof(Index));
        }

        // Dummy gateway: no live payment provider is wired up yet.
        var payment = new Payment
        {
            ApplicationUserId = userId,
            InvoiceId = invoice.Id,
            Reference = $"PAY-{DateTime.UtcNow:yyyyMM}-{Random.Shared.Next(10000, 99999)}",
            PaymentDate = DateTime.UtcNow,
            Amount = invoice.Balance,
            PaymentMethod = model.PaymentMethod,
            PaymentStatus = PaymentStatuses.Successful,
            CardLast4 = model.PaymentMethod == "Card" ? "4242" : null
        };

        _context.Payments.Add(payment);

        invoice.AmountPaid = invoice.Total;
        invoice.InvoiceStatus = InvoiceStatuses.Paid;

        var service = await _context.Services.FirstOrDefaultAsync(s => s.ApplicationUserId == userId, cancellationToken);
        if (service is not null)
        {
            service.OutstandingBalance = 0m;
        }

        await _context.SaveChangesAsync(cancellationToken);

        TempData["Success"] = $"Payment of R{payment.Amount:N2} received. Reference {payment.Reference}. Your invoice is now marked as paid.";
        return RedirectToAction(nameof(Invoice), new { id = invoice.Id });
    }

    private async Task<PaymentViewModel> BuildPaymentModelAsync(int? invoiceId, CancellationToken cancellationToken)
    {
        var userId = _userManager.GetUserId(User);

        var payable = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.ApplicationUserId == userId && i.Total - i.AmountPaid > 0)
            .OrderBy(i => i.DueDate)
            .ToListAsync(cancellationToken);

        return new PaymentViewModel { PayableInvoices = payable };
    }
}
