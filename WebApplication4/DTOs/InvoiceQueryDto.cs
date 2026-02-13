using WebApplication4.Models;
namespace WebApplication4.DTOs;

public class InvoiceQueryDto
{
    public Guid? CustomerId { get; set; }
    public InvoiceStatus? Status { get; set; }

    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string OrderBy { get; set; } = "CreatedAt";
    public bool Desc { get; set; } = true;
}