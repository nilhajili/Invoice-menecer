namespace WebApplication4.DTOs;

public class TokenResponseDto
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}