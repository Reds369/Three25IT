using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Three25IT.Models;

namespace Three25IT.Data;

public static class ApplicationDbContextSeed
{
    public const string DemoEmail = "demo@three25it.co.za";
    public const string DemoPassword = "Demo@12345";

    public static async Task SeedAsync(ApplicationDbContext context, IServiceProvider services)
    {
        await SeedPlansAsync(context);
        await SeedServiceAreasAsync(context);
        await SeedDemoCustomerAsync(context, services);
    }

    private static async Task SeedPlansAsync(ApplicationDbContext context)
    {
        if (await context.Plans.AnyAsync()) return;

        context.Plans.AddRange(
            new Plan
            {
                Name = "Fibre Starter 25", Slug = "fibre-starter-25",
                DownloadMbps = 25, UploadMbps = 25, MonthlyPrice = 349m,
                DataCap = null, IsUncapped = false,
                Description = "Affordable entry-level fibre for browsing, streaming and work-from-home.",
                IncludedFeatures = "25 Mbps down / 25 Mbps up|Uncapped night-time data (02:00 - 06:00)|Managed WiFi router included|24/7 support portal",
                ContractMonths = 12, SortOrder = 1
            },
            new Plan
            {
                Name = "Fibre Home 100", Slug = "fibre-home-100",
                DownloadMbps = 100, UploadMbps = 100, MonthlyPrice = 599m,
                DataCap = null, IsUncapped = true, IsFeatured = true,
                Description = "Our most popular home plan. Symmetrical speeds for streaming, gaming and HD calls.",
                IncludedFeatures = "100 Mbps down / 100 Mbps up (uncapped)|Free router & installation|4K streaming on 5 devices|24/7 support portal|Static IP available",
                ContractMonths = 12, SortOrder = 2
            },
            new Plan
            {
                Name = "Fibre Power 500", Slug = "fibre-power-500",
                DownloadMbps = 500, UploadMbps = 500, MonthlyPrice = 999m,
                DataCap = null, IsUncapped = true,
                Description = "Gigabit-ready speeds for power users, large families and heavy cloud workloads.",
                IncludedFeatures = "500 Mbps down / 500 Mbps up (uncapped)|Free router, ONT & installation|Static IP included|Priority support queue|Unlimited concurrent devices",
                ContractMonths = 12, SortOrder = 3
            },
            new Plan
            {
                Name = "Fibre Ultra 1000", Slug = "fibre-ultra-1000",
                DownloadMbps = 1000, UploadMbps = 1000, MonthlyPrice = 1499m,
                DataCap = null, IsUncapped = true,
                Description = "Our flagship symmetrical gigabit service with zero-compromise latency.",
                IncludedFeatures = "1000 Mbps down / 1000 Mbps up (uncapped)|Free 6E WiFi 7 router|Static IP included|Dedicated account manager|4-hour fault response SLA|99.9% uptime guarantee",
                ContractMonths = 24, SortOrder = 4
            },
            new Plan
            {
                Name = "Fibre Business Pro", Slug = "fibre-business-pro",
                DownloadMbps = 500, UploadMbps = 500, MonthlyPrice = 1799m,
                DataCap = null, IsUncapped = true, IsBusiness = true,
                Description = "Business-grade connectivity with SLA, redundancy options and priority fault handling.",
                IncludedFeatures = "500 Mbps symmetrical (uncapped)|99.9% uptime SLA|Static IP block of 4|Unlimited business support calls|Optional 4G/5G failover|Fixed monthly billing",
                ContractMonths = 24, SortOrder = 5
            }
        );

        await context.SaveChangesAsync();
    }

    private static async Task SeedServiceAreasAsync(ApplicationDbContext context)
    {
        if (await context.ServiceAreas.AnyAsync()) return;

        var areas = new List<ServiceArea>
        {
            new ServiceArea { Name = "Menlyn Node", Suburb = "Menlyn", City = "Pretoria", Province = "Gauteng", PostalCode = "0181", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "MEN-01", HomesPassed = 8420, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-4) },
            new ServiceArea { Name = "Brooklyn Node", Suburb = "Brooklyn", City = "Pretoria", Province = "Gauteng", PostalCode = "0181", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "BRK-02", HomesPassed = 6130, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-4) },
            new ServiceArea { Name = "Waterkloof Node", Suburb = "Waterkloof", City = "Pretoria", Province = "Gauteng", PostalCode = "0181", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 500, MaxUploadMbps = 500, ExchangeNode = "WTK-01", HomesPassed = 3980, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-9) },
            new ServiceArea { Name = "Garsfontein Node", Suburb = "Garsfontein", City = "Pretoria", Province = "Gauteng", PostalCode = "0042", CoverageStatus = CoverageStatuses.InProgress, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "GSF-03", HomesPassed = 5210, InstallFee = 250m, LastUpdated = DateTime.UtcNow.AddDays(-2), Notes = "Fibre splicing underway in phase 2 streets." },
            new ServiceArea { Name = "Centurion CBD", Suburb = "Centurion", City = "Centurion", Province = "Gauteng", PostalCode = "0157", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "CEN-01", HomesPassed = 11240, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-6) },
            new ServiceArea { Name = "Sunninghill Node", Suburb = "Sunninghill", City = "Randburg", Province = "Gauteng", PostalCode = "2191", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 500, MaxUploadMbps = 500, ExchangeNode = "SNH-02", HomesPassed = 4760, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-6) },
            new ServiceArea { Name = "Fourways Node", Suburb = "Fourways", City = "Sandton", Province = "Gauteng", PostalCode = "2191", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "FWR-01", HomesPassed = 9310, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-3) },
            new ServiceArea { Name = "Rosebank Node", Suburb = "Rosebank", City = "Johannesburg", Province = "Gauteng", PostalCode = "2196", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "RBK-01", HomesPassed = 7880, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-3) },
            new ServiceArea { Name = "Midrand Node", Suburb = "Midrand", City = "Midrand", Province = "Gauteng", PostalCode = "1685", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 500, MaxUploadMbps = 500, ExchangeNode = "MDR-01", HomesPassed = 8640, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-7) },
            new ServiceArea { Name = "Kempton Park Node", Suburb = "Kempton Park", City = "Ekurhuleni", Province = "Gauteng", PostalCode = "1619", CoverageStatus = CoverageStatuses.Planned, MaxDownloadMbps = 500, MaxUploadMbps = 500, PlannedMonth = 11, PlannedYear = DateTime.UtcNow.Year, ExchangeNode = "KTP-01", HomesPassed = 0, InstallFee = 350m, LastUpdated = DateTime.UtcNow.AddDays(-1), Notes = "Build scheduled to start this quarter." },
            new ServiceArea { Name = "Soweto Node", Suburb = "Soweto", City = "Johannesburg", Province = "Gauteng", PostalCode = "1808", CoverageStatus = CoverageStatuses.Waitlist, MaxDownloadMbps = 250, MaxUploadMbps = 250, PlannedMonth = 2, PlannedYear = DateTime.UtcNow.Year + 1, ExchangeNode = "SWT-01", HomesPassed = 0, InstallFee = 350m, LastUpdated = DateTime.UtcNow.AddDays(-1) },
            new ServiceArea { Name = "Table View Node", Suburb = "Table View", City = "Cape Town", Province = "Western Cape", PostalCode = "8018", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "TBV-01", HomesPassed = 9270, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-5) },
            new ServiceArea { Name = "Sea Point Node", Suburb = "Sea Point", City = "Cape Town", Province = "Western Cape", PostalCode = "8001", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 500, MaxUploadMbps = 500, ExchangeNode = "SPT-01", HomesPassed = 5490, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-5) },
            new ServiceArea { Name = "Durban North Node", Suburb = "Durban North", City = "Durban", Province = "KwaZulu-Natal", PostalCode = "4051", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 1000, MaxUploadMbps = 1000, ExchangeNode = "DBN-01", HomesPassed = 10120, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-8) },
            new ServiceArea { Name = "Umhlanga Node", Suburb = "Umhlanga", City = "eThekwini", Province = "KwaZulu-Natal", PostalCode = "4051", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 500, MaxUploadMbps = 500, ExchangeNode = "UHL-02", HomesPassed = 6340, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-8) },
            new ServiceArea { Name = "Bloemfontein Node", Suburb = "Bloemfontein", City = "Mangaung", Province = "Free State", PostalCode = "9301", CoverageStatus = CoverageStatuses.InProgress, MaxDownloadMbps = 500, MaxUploadMbps = 500, ExchangeNode = "BLO-01", HomesPassed = 4180, InstallFee = 250m, LastUpdated = DateTime.UtcNow.AddDays(-2) },
            new ServiceArea { Name = "Nelspruit Node", Suburb = "Nelspruit", City = "Mbombela", Province = "Mpumalanga", PostalCode = "1200", CoverageStatus = CoverageStatuses.Available, MaxDownloadMbps = 500, MaxUploadMbps = 500, ExchangeNode = "NLP-01", HomesPassed = 3720, InstallFee = 0m, LastUpdated = DateTime.UtcNow.AddDays(-10) },
            new ServiceArea { Name = "Polokwane Node", Suburb = "Polokwane", City = "Polokwane", Province = "Limpopo", PostalCode = "0700", CoverageStatus = CoverageStatuses.Planned, MaxDownloadMbps = 250, MaxUploadMbps = 250, PlannedMonth = 6, PlannedYear = DateTime.UtcNow.Year + 1, ExchangeNode = "PLK-01", HomesPassed = 0, InstallFee = 350m, LastUpdated = DateTime.UtcNow.AddDays(-1) }
        };

        context.ServiceAreas.AddRange(areas);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDemoCustomerAsync(ApplicationDbContext context, IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(DemoEmail);
        if (user is not null) return;

        user = new ApplicationUser
        {
            UserName = DemoEmail,
            Email = DemoEmail,
            EmailConfirmed = true,
            PhoneNumber = "+27 82 555 0125",
            FullName = "Thabo Molefe",
            StreetAddress = "42 Marula Street",
            Suburb = "Menlyn",
            City = "Pretoria",
            PostalCode = "0181",
            Province = "Gauteng",
            AccountNumber = "T3-2026-004821",
            IdNumber = "880415 5678 091",
            DateOfBirth = new DateTime(1988, 4, 15),
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await userManager.CreateAsync(user, DemoPassword);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Unable to seed the demo user: " + string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        var homePlan = await context.Plans.SingleAsync(p => p.Slug == "fibre-home-100");
        var ultraPlan = await context.Plans.SingleAsync(p => p.Slug == "fibre-ultra-1000");
        var now = DateTime.UtcNow;

        var service = new CustomerService
        {
            ApplicationUserId = user.Id,
            PlanId = homePlan.Id,
            AccountNumber = user.AccountNumber!,
            ServiceStatus = ServiceStatuses.Active,
            ActivatedOn = now.AddMonths(-14),
            ContractEndDate = now.AddMonths(10),
            NextBillingDate = now.AddDays(18),
            MonthlyPrice = homePlan.MonthlyPrice,
            FibreType = "Single-mode GPON, Cat6 drop",
            OntSerialNumber = "ONT-8842-7731-AX9",
            PublicIpAddress = "41.76.203.118",
            MacAddress = "3C:5A:B4:9F:12:7E",
            RouterModel = "Three25IT WiFi 6 Mesh (TG-M6)",
            InstallationAddress = "42 Marula Street, Menlyn, Pretoria, 0181",
            Notes = "ONT installed in the server cupboard. Customer requested a static IP retention on contract renewal.",
            OutstandingBalance = 0m
        };

        context.Services.Add(service);
        await context.SaveChangesAsync();

        SeedInvoices(context, user, service, now);
        SeedUsage(context, user, now);
        await SeedTickets(context, user, now);

        await context.SaveChangesAsync();
    }

    private static void SeedInvoices(ApplicationDbContext context, ApplicationUser user, CustomerService service, DateTime now)
    {
        var invoices = new List<Invoice>();

        for (int monthsAgo = 12; monthsAgo >= 1; monthsAgo--)
        {
            var periodStart = new DateTime(now.Year, now.Month, 1).AddMonths(-monthsAgo);
            var periodEnd = periodStart.AddMonths(1).AddDays(-1);
            var issue = periodStart.AddDays(0);
            var due = periodStart.AddDays(7);
            var isLatest = monthsAgo == 1;

            var lineItems = new List<InvoiceLineItem>
            {
                new InvoiceLineItem
                {
                    Description = $"Fibre Home 100 - {periodStart:MMMM yyyy}",
                    Quantity = 1,
                    UnitPrice = 599m,
                    Amount = 599m
                },
                new InvoiceLineItem
                {
                    Description = "Router rental (included)",
                    Quantity = 1,
                    UnitPrice = 0m,
                    Amount = 0m
                }
            };

            decimal extra = monthsAgo switch
            {
                3 => 149m,
                7 => 89m,
                _ => 0m
            };

            if (extra > 0)
            {
                lineItems.Add(new InvoiceLineItem
                {
                    Description = "Late payment handling fee",
                    Quantity = 1,
                    UnitPrice = extra,
                    Amount = extra
                });
            }

            var subtotal = lineItems.Sum(l => l.Amount);
            var vat = Math.Round(subtotal * 0.15m, 2);
            var total = subtotal + vat;
            var isPaid = !isLatest;

            invoices.Add(new Invoice
            {
                ApplicationUserId = user.Id,
                InvoiceNumber = $"INV-{periodStart:yyyyMM}-{4000 + (12 - monthsAgo)}",
                IssueDate = issue,
                DueDate = due,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd,
                Subtotal = subtotal,
                VAT = vat,
                Total = total,
                AmountPaid = isPaid ? total : 0m,
                InvoiceStatus = isPaid ? InvoiceStatuses.Paid : (due < now ? InvoiceStatuses.Overdue : InvoiceStatuses.Due),
                LineItems = lineItems
            });
        }

        context.Invoices.AddRange(invoices);
        context.SaveChanges();

        var paid = invoices.Where(i => i.InvoiceStatus == InvoiceStatuses.Paid).ToList();
        string[] methods = { "EFT", "Card", "EFT", "Card", "Card", "EFT", "Card", "EFT", "Card", "Card", "EFT" };

        for (int i = 0; i < paid.Count; i++)
        {
            context.Payments.Add(new Payment
            {
                ApplicationUserId = user.Id,
                InvoiceId = paid[i].Id,
                Reference = $"PAY-{paid[i].PeriodStart:yyyyMM}-{9000 + i}",
                PaymentDate = paid[i].IssueDate.AddDays(2 + (i % 4)),
                Amount = paid[i].Total,
                PaymentMethod = methods[i % methods.Length],
                PaymentStatus = PaymentStatuses.Successful,
                CardLast4 = methods[i % methods.Length] == "Card" ? new[] { "4242", "1881", "5309" }[i % 3] : null
            });
        }

        service.OutstandingBalance = invoices.Where(i => i.InvoiceStatus != InvoiceStatuses.Paid).Sum(i => i.Balance);
    }

    private static void SeedUsage(ApplicationDbContext context, ApplicationUser user, DateTime now)
    {
        var random = new Random(20260928);
        double[] download = { 412, 388, 501, 623, 355, 470, 540, 612, 498, 421, 566, 604 };
        double[] upload = { 88, 74, 121, 96, 66, 104, 132, 141, 110, 92, 128, 137 };

        for (int i = 0; i < 12; i++)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-(11 - i));

            context.UsageRecords.Add(new UsageRecord
            {
                ApplicationUserId = user.Id,
                Month = monthStart.ToString("MMMM yyyy"),
                MonthStart = monthStart,
                DownloadGb = (decimal)Math.Round(download[i] * (0.85 + random.NextDouble() * 0.3), 1),
                UploadGb = (decimal)Math.Round(upload[i] * (0.85 + random.NextDouble() * 0.3), 1),
                PeakDownloadMbps = (decimal)Math.Round(88 + random.NextDouble() * 11, 1),
                PeakUploadMbps = (decimal)Math.Round(79 + random.NextDouble() * 14, 1),
                AverageLatencyMs = (decimal)Math.Round(4 + random.NextDouble() * 6, 1)
            });
        }
    }

    private static async Task SeedTickets(ApplicationDbContext context, ApplicationUser user, DateTime now)
    {
        var open = new SupportTicket
        {
            ApplicationUserId = user.Id,
            TicketNumber = "TKT-2026-0142",
            Subject = "Intermittent drop in connection every evening around 19:00",
            Category = TicketCategories.Technical,
            Priority = TicketPriorities.High,
            TicketStatus = TicketStatuses.InProgress,
            CreatedAt = now.AddDays(-3),
            UpdatedAt = now.AddDays(-1),
            AssignedTo = "Technical Team - Fibre Node MEN-01"
        };

        open.Messages.Add(new TicketMessage
        {
            AuthorName = user.FullName ?? user.UserName!,
            AuthorUserId = user.Id,
            IsFromSupport = false,
            SentAt = now.AddDays(-3).AddHours(9),
            Body = "Hi team, for the past week the connection drops for roughly 5-10 minutes at around 19:00. It usually recovers on its own. Streaming and video calls are affected. Can you please check the line?"
        });
        open.Messages.Add(new TicketMessage
        {
            AuthorName = "Three25IT Support",
            IsFromSupport = true,
            SentAt = now.AddDays(-3).AddHours(11),
            Body = "Hi Thabo, thanks for reporting this. I've run a remote line diagnostic on your ONT and the fibre drop looks healthy. I've escalated to our node team to inspect the splitter port at MEN-01, as we can see elevated optical power loss in your segment during peak hours. You'll have an update within 24 hours."
        });
        open.Messages.Add(new TicketMessage
        {
            AuthorName = user.FullName ?? user.UserName!,
            AuthorUserId = user.Id,
            IsFromSupport = false,
            SentAt = now.AddDays(-1).AddHours(18),
            Body = "Thanks for the quick response. It happened again last night around 19:15. Happy to help with any remote diagnostics you need."
        });

        var billing = new SupportTicket
        {
            ApplicationUserId = user.Id,
            TicketNumber = "TKT-2026-0091",
            Subject = "Request static IP retention on contract renewal",
            Category = TicketCategories.Billing,
            Priority = TicketPriorities.Normal,
            TicketStatus = TicketStatuses.Resolved,
            CreatedAt = now.AddDays(-48),
            UpdatedAt = now.AddDays(-45),
            ClosedAt = now.AddDays(-45),
            AssignedTo = "Accounts Team"
        };

        billing.Messages.Add(new TicketMessage
        {
            AuthorName = user.FullName ?? user.UserName!,
            AuthorUserId = user.Id,
            IsFromSupport = false,
            SentAt = now.AddDays(-48).AddHours(14),
            Body = "When my 24-month contract ends I would like to keep the static IP address. Please confirm this won't increase my monthly fee."
        });
        billing.Messages.Add(new TicketMessage
        {
            AuthorName = "Three25IT Support",
            IsFromSupport = true,
            SentAt = now.AddDays(-45).AddHours(10),
            Body = "Good news Thabo - static IP retention is free of charge on Fibre Home 100. We've noted it on your account and it will carry over automatically at renewal. Nothing further needed from you."
        });

        var install = new SupportTicket
        {
            ApplicationUserId = user.Id,
            TicketNumber = "TKT-2025-0603",
            Subject = "Installation completed - ONT mounted in server cupboard",
            Category = TicketCategories.Installation,
            Priority = TicketPriorities.Low,
            TicketStatus = TicketStatuses.Closed,
            CreatedAt = now.AddMonths(-14).AddDays(-4),
            UpdatedAt = now.AddMonths(-14),
            ClosedAt = now.AddMonths(-14),
            AssignedTo = "Installations Team"
        };

        install.Messages.Add(new TicketMessage
        {
            AuthorName = "Three25IT Support",
            IsFromSupport = true,
            SentAt = now.AddMonths(-14).AddDays(-3).AddHours(8),
            Body = "Your installation is scheduled. Our technician will arrive between 09:00 and 12:00. Please ensure someone over 18 is available to provide access to the ONT location."
        });
        install.Messages.Add(new TicketMessage
        {
            AuthorName = user.FullName ?? user.UserName!,
            AuthorUserId = user.Id,
            IsFromSupport = false,
            SentAt = now.AddMonths(-14),
            Body = "Installation completed, everything tested and working. Great service, thanks Three25IT!"
        });

        context.SupportTickets.AddRange(open, billing, install);
        await context.SaveChangesAsync();
    }
}
