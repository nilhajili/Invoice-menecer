namespace WebApplication4.DTOs;

public class CreateInvoiceRowDto
{
    public string Service { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Amount { get; set; }
}