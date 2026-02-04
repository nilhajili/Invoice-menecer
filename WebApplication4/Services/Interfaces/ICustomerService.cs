using  WebApplication4.Models;
namespace WebApplication4.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(Guid id);
    Task<Customer> CreateAsync(Customer customer);
    Task<Customer?> UpdateAsync(Guid id, Customer customer);
    Task<bool> HardDeleteAsync(Guid id);
    Task<bool> ArchiveAsync(Guid id);
    
}