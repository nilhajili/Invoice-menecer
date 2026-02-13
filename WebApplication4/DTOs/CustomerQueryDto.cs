namespace WebApplication4.DTOs;

public class CustomerQueryDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public string OrderBy { get; set; } = "CreatedAt";
    public bool Desc { get; set; } = true;
}