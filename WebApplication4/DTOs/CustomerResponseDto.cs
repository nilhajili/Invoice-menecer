namespace WebApplication4.DTOs;

public class CustomerResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}