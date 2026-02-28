using System.Security.Claims;
using WebApplication4.Models;
using WebApplication4.DTOs;

namespace WebApplication4.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<PagedResult<Invoice>> GetListAsync(InvoiceQueryDto query);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<Invoice?> UpdateAsync(Guid id, Invoice invoice);
        Task<bool> HardDeleteAsync(Guid id);
        Task<bool> ArchiveAsync(Guid id);
        Task<bool> ChangeStatusAsync(
            ClaimsPrincipal user,
            Guid id,
            InvoiceStatus status);
        Task<Invoice?> GetInvoiceByIdAsync(Guid id);
    }
}