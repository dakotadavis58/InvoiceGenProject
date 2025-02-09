using InvoiceGeneratorAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using InvoiceGeneratorAPI.Data;
using InvoiceGeneratorAPI.DTOs.Auth;
using InvoiceGeneratorAPI.Exceptions;
using InvoiceGeneratorAPI.Common;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using InvoiceGeneratorAPI.Settings;
using BCrypt.Net;
using Google.Apis.Auth;
using InvoiceGeneratorAPI.DTOs.User;
using Microsoft.Extensions.Logging;

namespace InvoiceGeneratorAPI.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(ApplicationDbContext context, IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<User> RegisterUserAsync(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            throw new DuplicateEmailException("Email already registered");
        }

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.IsAdmin ? Roles.Admin : Roles.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<(string AccessToken, string RefreshToken, DTOs.User.UserDto User)> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", request.Email);
                throw new AuthenticationException("Invalid email or password");
            }

            _logger.LogDebug("Verifying password for user: {UserId}", user.Id);

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for user: {UserId}", user.Id);
                throw new AuthenticationException("Invalid email or password");
            }

            _logger.LogDebug("Generating tokens for user: {UserId}", user.Id);
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Login successful for user: {UserId}", user.Id);

            return (
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                User: MapToDto(user)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for email: {Email}", request.Email);
            throw;
        }
    }

    public async Task<(string AccessToken, string RefreshToken, DTOs.User.UserDto User)> RefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new AuthenticationException("Invalid refresh token");
        }

        var accessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        await _context.SaveChangesAsync();

        return (
            AccessToken: accessToken,
            RefreshToken: newRefreshToken,
            User: MapToDto(user)
        );
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsEmailAvailableAsync(string email)
    {
        return !await _context.Users.AnyAsync(u => u.Email == email);
    }

    public async Task<(string AccessToken, string RefreshToken, DTOs.User.UserDto User)> HandleGoogleAuthAsync(GoogleJsonWebSignature.Payload payload)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleId == payload.Subject || u.Email == payload.Email);

        if (user == null)
        {
            // Create new user
            user = new User
            {
                Email = payload.Email,
                FirstName = payload.GivenName,
                LastName = payload.FamilyName,
                GoogleId = payload.Subject,
                EmailConfirmed = payload.EmailVerified,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
        }
        else
        {
            // Update existing user
            user.GoogleId = payload.Subject;
            user.FirstName = payload.GivenName;
            user.LastName = payload.FamilyName;
            user.EmailConfirmed = payload.EmailVerified;
            user.UpdatedAt = DateTime.UtcNow;
        }

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);

        await _context.SaveChangesAsync();

        return (
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            User: MapToDto(user)
        );
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return null;

        // Generate a random token
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        user.RefreshToken = token; // Reusing RefreshToken field for password reset
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(24); // Token expires in 24 hours
        await _context.SaveChangesAsync();

        return token;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == email &&
            u.RefreshToken == token &&
            u.RefreshTokenExpiryTime > DateTime.UtcNow);

        if (user == null)
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }
}