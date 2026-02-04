using WebApplication4.Services.Interfaces;
using WebApplication4.Models;
using WebApplication4.Data;
using Microsoft.EntityFrameworkCore;
namespace WebApplication4.Services;

public class InvoiceService : IInvoiceService
{
     private readonly InvoiceDbContext _context;

    public InvoiceService(InvoiceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
        => await _context.Invoices
            .Include(i => i.Rows)
            .ToListAsync();

    public async Task<Invoice?> GetByIdAsync(Guid id)
        => await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Invoice> CreateAsync(Invoice invoice)
    {
        invoice.Id = Guid.NewGuid();
        invoice.CreatedAt = DateTimeOffset.UtcNow;
        invoice.TotalSum = invoice.Rows.Sum(r => r.Sum);
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<Invoice?> UpdateAsync(Guid id, Invoice invoice)
    {
        var existing = await _context.Invoices
            .Include(i => i.Rows)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (existing == null || existing.Status != InvoiceStatus.Created) return null;

        existing.StartDate = invoice.StartDate;
        existing.EndDate = invoice.EndDate;
        existing.Rows = invoice.Rows;
        existing.TotalSum = invoice.Rows.Sum(r => r.Sum);
        existing.Comment = invoice.Comment;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> ChangeStatusAsync(Guid id, InvoiceStatus newStatus)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null || invoice.DeletedAt != null)
            return false; 
        invoice.Status = newStatus;
        invoice.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return true; 
    }

    public async Task<bool> HardDeleteAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null || invoice.Status != InvoiceStatus.Created) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice == null) return false;

        invoice.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}