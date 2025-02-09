using Microsoft.AspNetCore.Mvc;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Auth;
using InvoiceGeneratorAPI.DTOs.User;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using InvoiceGeneratorAPI.Settings;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using InvoiceGeneratorAPI.Extensions;

namespace InvoiceGeneratorAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    IConfiguration configuration,
    IOptions<GoogleAuthSettings> googleAuthSettings,
    IWebHostEnvironment environment,
    IEmailService emailService) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private const string RefreshTokenCookie = "refreshToken";
    private readonly IConfiguration _configuration = configuration;
    private readonly GoogleAuthSettings _googleAuthSettings = googleAuthSettings.Value;
    private readonly IWebHostEnvironment _environment = environment;
    private readonly IEmailService _emailService = emailService;

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterRequest request)
    {
        var user = await _authService.RegisterUserAsync(request);
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
        };

        return Created($"/api/users/{user.Id}", userDto);
    }

    private static AuthResponse CreateAuthResponse(string token, UserDto userDto)
    {
        return new AuthResponse
        {
            Token = token,
            User = userDto  // Now we can directly use UserDto since there's only one definition
        };
    }

    private void SetTokenCookies(string accessToken, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // for HTTPS
            SameSite = SameSiteMode.Strict,
        };

        // Set refresh token to expire in 7 days
        Response.Cookies.Append(RefreshTokenCookie, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        // Set access token to expire in 15 minutes
        Response.Cookies.Append("token", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var (accessToken, refreshToken, userDto) = await _authService.LoginAsync(request);
        SetTokenCookies(accessToken, refreshToken);
        return Ok(CreateAuthResponse(accessToken, userDto));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshToken()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized("No refresh token provided");

        var (accessToken, newRefreshToken, userDto) = await _authService.RefreshTokenAsync(refreshToken);
        SetTokenCookies(accessToken, newRefreshToken);
        return Ok(CreateAuthResponse(accessToken, userDto));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookie];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _authService.RevokeRefreshTokenAsync(refreshToken);
        }

        Response.Cookies.Delete(RefreshTokenCookie);
        Response.Cookies.Delete("token");  // Also delete the access token cookie
        return Ok();
    }

    [HttpGet("email-available")]
    public async Task<ActionResult<bool>> IsEmailAvailable([FromQuery] string email)
    {
        var isAvailable = await _authService.IsEmailAvailableAsync(email);
        return Ok(isAvailable);
    }

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponse>> GoogleAuth([FromBody] GoogleAuthRequest request)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleAuthSettings.ClientId }
            });

            var (token, refreshToken, user) = await _authService.HandleGoogleAuthAsync(payload);

            // Set refresh token in HTTP-only cookie
            SetTokenCookies(token, refreshToken);

            return Ok(new AuthResponse
            {
                Token = token,  // Changed from AccessToken
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            });
        }
        catch (InvalidJwtException)
        {
            return BadRequest("Invalid Google token");
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = User.GetUserId();
        var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

        if (!result)
            return BadRequest("Current password is incorrect");

        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var token = await _authService.GeneratePasswordResetTokenAsync(request.Email);
        if (token == null)
            return Ok(); // Don't reveal if email exists

        var encodedToken = HttpUtility.UrlEncode(token);
        var resetLink = $"{_configuration["AppUrl"]}/reset-password?token={encodedToken}&email={HttpUtility.UrlEncode(request.Email)}";

        if (_environment.IsDevelopment())
        {
            // In development, also return the link for testing
            await _emailService.SendPasswordResetEmailAsync(request.Email, resetLink);
            return Ok(new { resetLink });
        }

        await _emailService.SendPasswordResetEmailAsync(request.Email, resetLink);
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request.Email, request.Token, request.Password);

        if (!result)
            return BadRequest("Invalid or expired password reset token");

        return Ok();
    }
}