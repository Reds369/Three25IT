using Microsoft.EntityFrameworkCore;
using Three25IT.Data;
using Three25IT.Models;
using Three25IT.ViewModels;

namespace Three25IT.Services;

public interface ICoverageService
{
    Task<CoverageCheckViewModel> CheckAsync(string? address);
}

public class CoverageService : ICoverageService
{
    private readonly ApplicationDbContext _context;

    public CoverageService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CoverageCheckViewModel> CheckAsync(string? address)
    {
        var result = new CoverageCheckViewModel { Address = address ?? string.Empty };

        if (string.IsNullOrWhiteSpace(address) || address.Trim().Length < 3)
        {
            return result;
        }

        var term = Normalise(address);
        result.Normalised = term;
        result.Searched = true;

        var areas = await _context.ServiceAreas.AsNoTracking().ToListAsync();

        result.Matches = areas
            .Where(a => Matches(a, term))
            .OrderBy(a => a.CoverageStatus == CoverageStatuses.Available ? 0 : 1)
            .ThenByDescending(a => a.MaxDownloadMbps)
            .ToList();

        result.Primary = result.Matches.FirstOrDefault();

        var plans = await _context.Plans
            .AsNoTracking()
            .OrderBy(p => p.SortOrder)
            .ToListAsync();

        if (result.Primary is not null)
        {
            result.EligiblePlans = plans
                .Where(p => p.DownloadMbps <= result.Primary.MaxDownloadMbps)
                .ToList();

            result.ResultType = result.Primary.CoverageStatus switch
            {
                CoverageStatuses.Available => "available",
                CoverageStatuses.InProgress => "inprogress",
                CoverageStatuses.Planned => "planned",
                CoverageStatuses.Waitlist => "waitlist",
                _ => "none"
            };

            var area = result.Primary;
            var speeds = $"{area.MaxDownloadMbps} Mbps down / {area.MaxUploadMbps} Mbps up";

            result.ResultTitle = area.CoverageStatus switch
            {
                CoverageStatuses.Available =>
                    $"Great news - fibre is available at {area.Suburb}",
                CoverageStatuses.InProgress =>
                    $"Fibre is being built in {area.Suburb} right now",
                CoverageStatuses.Planned =>
                    $"{area.Suburb} is on our build plan",
                CoverageStatuses.Waitlist =>
                    $"{area.Suburb} is on our waitlist",
                _ => "We could not find that address"
            };

            var months = new[] { "January", "February", "March", "April", "May", "June",
                                 "July", "August", "September", "October", "November", "December" };

            result.ResultMessage = area.CoverageStatus switch
            {
                CoverageStatuses.Available =>
                    $"We can connect {area.Suburb} at up to {speeds}. Book a free installation and your service goes live in as little as 3 business days.",
                CoverageStatuses.InProgress =>
                    $"Our team is splicing fibre in {area.Suburb} as we speak, with {area.HomesPassed:N0} homes passed on this node. Speeds of up to {speeds} are expected. Leave your details and we'll call you the moment your street goes live.",
                CoverageStatuses.Planned =>
                    $"{area.Suburb} is scheduled for our next build phase, targeting {months[(area.PlannedMonth ?? 1) - 1]} {area.PlannedYear}. Register your interest and we'll notify you the day fibre reaches your door.",
                CoverageStatuses.Waitlist =>
                    $"There's high demand in {area.Suburb}. Join the waitlist and we'll contact you as soon as a build slot opens up or your street reaches an active node.",
                _ => "Please check the address and try again."
            };
        }
        else
        {
            result.ResultType = "none";
            result.ResultTitle = "We could not find that address";
            result.ResultMessage =
                "That address isn't in our coverage database yet. Fibre buildouts move quickly - leave your details and our sales team will confirm as soon as we plan your suburb. You can also reach us on 0860 000 325 to check manually.";
        }

        return result;
    }

    private static bool Matches(ServiceArea area, string term)
    {
        return Contains(area.Name, term)
            || Contains(area.Suburb, term)
            || Contains(area.City, term)
            || Contains(area.Province, term)
            || Contains(area.PostalCode, term)
            || Contains(area.ExchangeNode, term);
    }

    private static bool Contains(string? value, string term)
    {
        return !string.IsNullOrWhiteSpace(value)
            && Normalise(value).Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static string Normalise(string value)
    {
        return value.Trim().ToLowerInvariant();
    }
}
