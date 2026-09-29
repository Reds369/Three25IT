using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.Services;
using Three25IT.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Three25IT.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICoverageService _coverage;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(
        ApplicationDbContext context,
        ICoverageService coverage,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _coverage = coverage;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? address, CancellationToken cancellationToken)
    {
        var model = new HomeViewModel
        {
            Plans = await _context.Plans
                .AsNoTracking()
                .Where(p => !p.IsBusiness)
                .OrderBy(p => p.SortOrder)
                .ToListAsync(cancellationToken)
        };

        var areas = await _context.ServiceAreas
            .AsNoTracking()
            .Where(a => a.CoverageStatus == CoverageStatuses.Available)
            .ToListAsync(cancellationToken);

        model.SuburbsCovered = areas.Select(a => a.Suburb).Distinct().Count();
        model.HomesPassed = areas.Sum(a => a.HomesPassed);

        if (!string.IsNullOrWhiteSpace(address))
        {
            model.Coverage = await _coverage.CheckAsync(address);
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Coverage(string? address, CancellationToken cancellationToken)
    {
        var model = await _coverage.CheckAsync(address);

        ViewBag.SuburbsCovered = await _context.ServiceAreas
            .AsNoTracking()
            .Where(a => a.CoverageStatus == CoverageStatuses.Available)
            .Select(a => a.Suburb)
            .Distinct()
            .CountAsync(cancellationToken);

        ViewBag.HomesPassed = await _context.ServiceAreas
            .AsNoTracking()
            .Where(a => a.CoverageStatus == CoverageStatuses.Available)
            .SumAsync(a => (decimal?)a.HomesPassed, cancellationToken) ?? 0m;

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Pricing(string? plan, CancellationToken cancellationToken)
    {
        return View(new PricingViewModel
        {
            HighlightSlug = plan,
            Plans = await _context.Plans
                .AsNoTracking()
                .OrderBy(p => p.SortOrder)
                .ToListAsync(cancellationToken)
        });
    }

    [HttpGet]
    public async Task<IActionResult> Contact(int? plan, string? address, CancellationToken cancellationToken)
    {
        var plans = await _context.Plans
            .AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        var model = new ContactViewModel
        {
            PlanId = plan,
            CoverageResult = address,
            PlanOptions = plans
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} - {p.DownloadMbps} Mbps ({p.MonthlyPrice:0} per month)"
                })
                .ToList()
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User), cancellationToken);

            if (user is not null)
            {
                model.FullName = user.FullName ?? string.Empty;
                model.Email = user.Email ?? string.Empty;
                model.Phone = user.PhoneNumber ?? string.Empty;
                model.Address = string.Join(", ", new[] { user.StreetAddress, user.Suburb, user.City, user.PostalCode }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactViewModel model, CancellationToken cancellationToken)
    {
        var plans = await _context.Plans
            .AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .ToListAsync(cancellationToken);

        if (ModelState.IsValid)
        {
            var enquiry = new Enquiry
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                PlanId = model.PlanId,
                Message = model.Message,
                CoverageResult = model.CoverageResult,
                CreatedAt = DateTime.UtcNow
            };

            _context.Enquiries.Add(enquiry);
            await _context.SaveChangesAsync(cancellationToken);

            return View("EnquirySuccess", new EnquirySuccessViewModel
            {
                Reference = $"ENQ-{DateTime.UtcNow:yyyyMM}-{enquiry.Id:00000}",
                FullName = model.FullName,
                CoverageWasAvailable = !string.IsNullOrWhiteSpace(model.CoverageResult)
                    && model.CoverageResult.Contains("available", StringComparison.OrdinalIgnoreCase)
            });
        }

        model.PlanOptions = plans
            .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} - {p.DownloadMbps} Mbps ({p.MonthlyPrice:0} per month)"
            })
            .ToList();

        return View(model);
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = HttpContext.TraceIdentifier
        });
    }
}
