namespace WebApplication4.DTOs;

public class UpdateInvoiceDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public List<UpdateInvoiceRowDto> Rows { get; set; } = new();
    public string? Comment { get; set; }
}