using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.Models;

namespace InvoiceGeneratorAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // User -> Company relationship
        modelBuilder.Entity<Company>()
            .HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Company>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Company -> Client relationship
        modelBuilder.Entity<Client>()
            .HasOne(c => c.Company)
            .WithMany()
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Company -> Invoice relationship
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Company)
            .WithMany()
            .HasForeignKey(i => i.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Invoice -> InvoiceItem relationship
        modelBuilder.Entity<InvoiceItem>()
            .HasOne(i => i.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(i => i.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}