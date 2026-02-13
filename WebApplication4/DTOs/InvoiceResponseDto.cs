using WebApplication4.Models;
namespace WebApplication4.DTOs;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public decimal TotalSum { get; set; }
    public InvoiceStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public List<InvoiceRowResponseDto> Rows { get; set; } = [];
}