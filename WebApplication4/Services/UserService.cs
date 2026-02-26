using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication4.Data;
using WebApplication4.DTOs;
using WebApplication4.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserService : IUserService
{
    private readonly UserDbContext _context;
    private readonly PasswordHasher<User> _hasher;
    private readonly IConfiguration _config;

    public UserService(UserDbContext context, IConfiguration config)
    {
        _context = context;
        _hasher = new PasswordHasher<User>();
        _config = config;
    }

    public async Task RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(x => x.Email == dto.Email))
            throw new Exception("Email already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<TokenResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            throw new Exception("Invalid email or password");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Invalid email or password");

        return GenerateTokens(user);
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task UpdateProfileAsync(Guid id, UpdateProfileDto dto)
    {
        var user = await GetByIdAsync(id);
        if (user == null) throw new Exception("User not found");

        user.Name = dto.Name;
        user.Address = dto.Address;
        user.PhoneNumber = dto.PhoneNumber;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task ChangePasswordAsync(Guid id, ChangePasswordDto dto)
    {
        var user = await GetByIdAsync(id);
        if (user == null) throw new Exception("User not found");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Old password is incorrect");

        user.PasswordHash = _hasher.HashPassword(user, dto.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteOwnProfileAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null) throw new Exception("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string token, string refreshToken)
    {
        var key = _config["Jwt:Key"];
        var issuer = _config["Jwt:Issuer"];

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(issuer))
            throw new Exception("JWT configuration is missing!");

        var principal = JwtHelper.GetPrincipalFromExpiredToken(token, key, issuer);

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            throw new SecurityTokenException("Invalid token");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null ||
            user.RefreshToken != refreshToken ||
            user.RefreshTokenExpiryTime <= DateTimeOffset.UtcNow)
        {
            throw new SecurityTokenException("Invalid refresh token");
        }

        return GenerateTokens(user);
    }

    private TokenResponseDto GenerateTokens(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                new Claim(ClaimTypes.Name, user.Id.ToString()),           
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["Jwt:Issuer"], 
            Audience = _config["Jwt:Audience"] 
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        var refreshToken = Guid.NewGuid().ToString();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTimeOffset.UtcNow.AddDays(7);

        _context.SaveChanges();

        return new TokenResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            RefreshToken = refreshToken
        };
    }
}