using WebApplication4.DTOs;
using WebApplication4.Models;

public interface IUserService
{
    Task RegisterAsync(RegisterDto dto);
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
    Task<User?> GetByIdAsync(Guid id);
    Task UpdateProfileAsync(Guid id, UpdateProfileDto dto);
    Task ChangePasswordAsync(Guid id, ChangePasswordDto dto);
    Task DeleteOwnProfileAsync(Guid id);
    Task<TokenResponseDto> RefreshTokenAsync(string token, string refreshToken);
}