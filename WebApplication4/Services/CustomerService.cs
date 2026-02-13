using WebApplication4.Services.Interfaces;
using WebApplication4.Models;
using WebApplication4.Data;
using WebApplication4.DTOs;
using Microsoft.EntityFrameworkCore;
namespace WebApplication4.Services;

public class CustomerService : ICustomerService
{
    private readonly InvoiceDbContext _context;

    public CustomerService(InvoiceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
        => await _context.Customers.ToListAsync();

    public async Task<Customer?> GetByIdAsync(Guid id)
        => await _context.Customers.FindAsync(id);

    public async Task<Customer> CreateAsync(Customer customer)
    {
        customer.Id = Guid.NewGuid();
        customer.CreatedAt = DateTimeOffset.UtcNow;
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }
    public async Task<PagedResult<Customer>> GetListAsync(CustomerQueryDto query)
    {
        IQueryable<Customer> customers = _context.Customers;

        if (!string.IsNullOrWhiteSpace(query.Name))
            customers = customers.Where(x => x.Name.Contains(query.Name));

        if (!string.IsNullOrWhiteSpace(query.Email))
            customers = customers.Where(x => x.Email.Contains(query.Email));

        customers = query.OrderBy switch
        {
            "Name" => query.Desc ? customers.OrderByDescending(x => x.Name) : customers.OrderBy(x => x.Name),
            _ => query.Desc ? customers.OrderByDescending(x => x.CreatedAt) : customers.OrderBy(x => x.CreatedAt)
        };

        int total = await customers.CountAsync();

        var items = await customers
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<Customer>
        {
            Items = items,
            TotalCount = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<Customer?> UpdateAsync(Guid id, Customer customer)
    {
        var existing = await _context.Customers.FindAsync(id);
        if (existing == null || existing.DeletedAt != null) return null;

        existing.Name = customer.Name;
        existing.Address = customer.Address;
        existing.Email = customer.Email;
        existing.PhoneNumber = customer.PhoneNumber;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> HardDeleteAsync(Guid id)
    {
        var customer = await _context.Customers
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null || customer.Invoices.Any()) return false;

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ArchiveAsync(Guid id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return false;

        customer.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }  
}