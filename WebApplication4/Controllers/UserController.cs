using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication4.DTOs;
using WebApplication4.Services.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    public UserController(IUserService service) => _service = service;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _service.RegisterAsync(dto);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var token = await _service.LoginAsync(dto);
            return Ok(token);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenResponseDto dto)
    {
        var token = await _service.RefreshTokenAsync(dto.Token, dto.RefreshToken);
        return Ok(token);
    }

    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized("User not authenticated");

        if (!Guid.TryParse(userIdString, out var userId))
            return BadRequest("Invalid user ID in token");

        await _service.UpdateProfileAsync(userId, dto);
        return Ok("Profile updated successfully");
    }

    [HttpPut("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    
        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized("User not authenticated");

        if (!Guid.TryParse(userIdString, out var userId))
            return BadRequest("Invalid user ID in token");

        await _service.ChangePasswordAsync(userId, dto);
        return Ok("Password changed successfully");
    }

    [HttpDelete("me")]
    [Authorize] 
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        await _service.DeleteOwnProfileAsync(userId);
        return Ok();
    }
}