using InvoiceGeneratorAPI.DTOs.Auth;
using InvoiceGeneratorAPI.DTOs.User;
using InvoiceGeneratorAPI.Models;
using Google.Apis.Auth;

namespace InvoiceGeneratorAPI.Services;

public interface IAuthService
{
    Task<User> RegisterUserAsync(RegisterRequest request);
    Task<bool> IsEmailAvailableAsync(string email);
    Task<(string AccessToken, string RefreshToken, DTOs.User.UserDto User)> LoginAsync(LoginRequest request);
    Task<(string AccessToken, string RefreshToken, DTOs.User.UserDto User)> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
    Task<(string AccessToken, string RefreshToken, DTOs.User.UserDto User)> HandleGoogleAuthAsync(GoogleJsonWebSignature.Payload payload);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<string?> GeneratePasswordResetTokenAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
}