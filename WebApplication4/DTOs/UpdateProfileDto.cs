namespace WebApplication4.DTOs;


public class UpdateProfileDto
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}