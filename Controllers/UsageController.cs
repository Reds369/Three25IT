using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Controllers;

[Authorize]
[Route("Usage")]
public class UsageController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UsageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        var records = await _context.UsageRecords
            .AsNoTracking()
            .Where(u => u.ApplicationUserId == userId)
            .OrderByDescending(u => u.MonthStart)
            .Take(12)
            .ToListAsync(cancellationToken);

        var model = new UsageViewModel
        {
            Service = service,
            Plan = service?.Plan,
            Records = records,
            TotalDownloadGb = records.Sum(r => r.DownloadGb),
            TotalUploadGb = records.Sum(r => r.UploadGb),
            AvgDownloadGb = records.Count > 0 ? Math.Round(records.Average(r => r.DownloadGb), 1) : 0m,
            PeakDownloadMbps = records.Count > 0 ? records.Max(r => r.PeakDownloadMbps) : 0m
        };

        return View(model);
    }
}
