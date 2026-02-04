using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using WebApplication4.Models;

namespace WebApplication4.Data;

public class InvoiceDbContext:DbContext
{
    public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) 
        : base(options)
    {
        
    }
    public DbSet<Invoice> Invoices=>Set<Invoice>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Customer>(proj =>
        {
            proj.HasKey(c => c.Id);
            proj.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            proj.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(50);
            proj.HasIndex(c => c.Email)
                .IsUnique();
            proj.Property(c => c.PhoneNumber)
                .HasMaxLength(50);
            proj.Property(c => c.Address)
                .HasMaxLength(100);
            proj.Property(c => c.CreatedAt)
                .IsRequired();
            proj.HasQueryFilter(c => c.DeletedAt == null);
        });
        modelBuilder.Entity<Invoice>(proj =>
        {
            proj.HasKey(i => i.Id);
            proj.HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            proj.Property(i => i.StartDate)
                .IsRequired();
            proj.Property(i => i.EndDate)
                .IsRequired();
            proj.Property(i => i.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
            proj.Property(i => i.TotalSum)
                .HasPrecision(18, 2)
                .IsRequired();
            proj.Property(i => i.Comment)
                .HasMaxLength(500);
            proj.Property(i => i.CreatedAt)
                .IsRequired();
            proj.Property(i => i.UpdatedAt);
            proj.Property(i => i.DeletedAt);
            proj.HasQueryFilter(i => i.DeletedAt == null);
        });
        modelBuilder.Entity<InvoiceRow>(proj =>
        {
            proj.HasKey(r => r.Id);
            proj.HasOne(r => r.Invoice)
                .WithMany(i => i.Rows)
                .HasForeignKey(r => r.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            proj.Property(r => r.Service)
                .IsRequired()
                .HasMaxLength(200);
            proj.Property(r => r.Quantity)
                .IsRequired()
                .HasPrecision(18, 4);
            proj.Property(r => r.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
            proj.Property(r => r.Sum)
                .HasPrecision(18, 2)
                .IsRequired();
        });
    }

}