namespace WebApplication4.Models;

public class Invoice
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; }

    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }

    public ICollection<InvoiceRow> Rows { get; set; } = new List<InvoiceRow>();

    public decimal TotalSum { get; set; }

    public string? Comment { get; set; }

    public InvoiceStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

public enum InvoiceStatus
{
    Created,
    Sent, 
    Received, 
    Paid, 
    Cancelled, 
    Rejected
}