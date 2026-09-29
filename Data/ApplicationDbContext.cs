using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Three25IT.Models;

namespace Three25IT.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<CustomerService> Services => Set<CustomerService>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceLineItem> InvoiceLineItems => Set<InvoiceLineItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();
    public DbSet<UsageRecord> UsageRecords => Set<UsageRecord>();
    public DbSet<ServiceArea> ServiceAreas => Set<ServiceArea>();
    public DbSet<Enquiry> Enquiries => Set<Enquiry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Plan>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired();
            entity.Property(p => p.Slug).IsRequired();
            entity.Property(p => p.MonthlyPrice).HasColumnType("decimal(18,2)");
        });

        builder.Entity<CustomerService>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.AccountNumber).IsRequired();
            entity.Property(s => s.MonthlyPrice).HasColumnType("decimal(18,2)");
            entity.Property(s => s.OutstandingBalance).HasColumnType("decimal(18,2)");

            entity.HasIndex(s => s.AccountNumber).IsUnique();
            entity.HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Service)
                .HasForeignKey<CustomerService>(s => s.ApplicationUserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(s => s.Plan)
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Invoice>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.InvoiceNumber).IsRequired();
            entity.Property(i => i.Subtotal).HasColumnType("decimal(18,2)");
            entity.Property(i => i.VAT).HasColumnType("decimal(18,2)");
            entity.Property(i => i.Total).HasColumnType("decimal(18,2)");
            entity.Property(i => i.AmountPaid).HasColumnType("decimal(18,2)");

            entity.HasIndex(i => i.InvoiceNumber).IsUnique();
            entity.HasOne(i => i.ApplicationUser)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<InvoiceLineItem>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(l => l.Amount).HasColumnType("decimal(18,2)");

            entity.HasOne(l => l.Invoice)
                .WithMany(i => i.LineItems)
                .HasForeignKey(l => l.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Payment>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Reference).IsRequired();
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");

            entity.HasIndex(p => p.Reference).IsUnique();
            entity.HasOne(p => p.ApplicationUser)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<SupportTicket>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.TicketNumber).IsRequired();

            entity.HasIndex(t => t.TicketNumber).IsUnique();
            entity.HasOne(t => t.ApplicationUser)
                .WithMany(u => u.Tickets)
                .HasForeignKey(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(t => t.Messages)
                .WithOne(m => m.SupportTicket!)
                .HasForeignKey(m => m.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TicketMessage>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.HasOne(m => m.SupportTicket)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.SupportTicketId);
        });

        builder.Entity<UsageRecord>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.DownloadGb).HasColumnType("decimal(18,2)");
            entity.Property(u => u.UploadGb).HasColumnType("decimal(18,2)");
            entity.Property(u => u.PeakDownloadMbps).HasColumnType("decimal(18,2)");
            entity.Property(u => u.PeakUploadMbps).HasColumnType("decimal(18,2)");
            entity.Property(u => u.AverageLatencyMs).HasColumnType("decimal(18,2)");

            entity.HasOne(u => u.ApplicationUser)
                .WithMany(a => a.Usage)
                .HasForeignKey(u => u.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<ServiceArea>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired();
            entity.Property(a => a.Suburb).IsRequired();
            entity.Property(a => a.City).IsRequired();
            entity.Property(a => a.Province).IsRequired();
            entity.Property(a => a.PostalCode).IsRequired();
            entity.Property(a => a.InstallFee).HasColumnType("decimal(18,2)");

            entity.HasIndex(a => a.PostalCode);
            entity.HasIndex(a => a.Suburb);
        });

        builder.Entity<Enquiry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Plan)
                .WithMany()
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
