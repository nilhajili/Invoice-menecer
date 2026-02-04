using WebApplication4.Models;

namespace WebApplication4.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<Invoice> CreateAsync(Invoice invoice);
        Task<Invoice?> UpdateAsync(Guid id, Invoice invoice);
        Task<bool> HardDeleteAsync(Guid id);
        Task<bool> ArchiveAsync(Guid id);
        Task<bool> ChangeStatusAsync(Guid id, InvoiceStatus status);
    }
}