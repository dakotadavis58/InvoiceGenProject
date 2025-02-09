using Microsoft.EntityFrameworkCore;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Auth;
using InvoiceGeneratorAPI.Exceptions;
using InvoiceGeneratorAPI.Settings;
using FluentAssertions;
using InvoiceGeneratorAPI.Tests;
using Xunit;
using InvoiceGeneratorAPI.Models;
using Microsoft.Extensions.Options;

namespace InvoiceGeneratorAPI.Tests.Services;

public class AuthServiceTests : TestBase
{
    private readonly IAuthService _authService;
    private readonly IOptions<JwtSettings> _jwtSettings;

    public AuthServiceTests()
    {
        _jwtSettings = Options.Create(new JwtSettings
        {
            SecretKey = "your-super-secret-key-that-is-at-least-32-bytes-long-for-testing",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationInMinutes = 60,
            RefreshTokenExpirationInDays = 7
        });

        _authService = new AuthService(Context, _jwtSettings);
    }

    [Fact]
    public async Task RegisterUserAsync_WithValidRequest_ShouldCreateUser()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await _authService.RegisterUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.FirstName.Should().Be(request.FirstName);
        result.LastName.Should().Be(request.LastName);
        result.PasswordHash.Should().NotBe(request.Password); // Password should be hashed
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WithUnusedEmail_ShouldReturnTrue()
    {
        // Arrange
        var email = "unused@example.com";

        // Act
        var result = await _authService.IsEmailAvailableAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsEmailAvailableAsync_WithExistingEmail_ShouldReturnFalse()
    {
        // Arrange
        var email = "existing@example.com";
        var user = new User { Email = email, PasswordHash = "hash" };
        await Context.Users.AddAsync(user);
        await Context.SaveChangesAsync();

        // Act
        var result = await _authService.IsEmailAvailableAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };
        await _authService.RegisterUserAsync(registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var (accessToken, refreshToken, user) = await _authService.LoginAsync(loginRequest);

        // Assert
        accessToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();
        user.Should().NotBeNull();
        user.Email.Should().Be(loginRequest.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldThrowAuthenticationException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "Password123!"
        };

        // Act
        var action = () => _authService.LoginAsync(loginRequest);

        // Assert
        await action.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowAuthenticationException()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };
        await _authService.RegisterUserAsync(registerRequest);

        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var action = () => _authService.LoginAsync(loginRequest);

        // Assert
        await action.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };
        await _authService.RegisterUserAsync(registerRequest);

        var (_, initialRefreshToken, _) = await _authService.LoginAsync(new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        });

        // Wait a short time to ensure tokens are different
        await Task.Delay(1000);

        // Act
        var (newAccessToken, newRefreshToken, newUser) = await _authService.RefreshTokenAsync(initialRefreshToken);

        // Assert
        newAccessToken.Should().NotBeNullOrEmpty();
        newRefreshToken.Should().NotBeNullOrEmpty();
        newRefreshToken.Should().NotBe(initialRefreshToken);
        newUser.Should().NotBeNull();
        newUser.Email.Should().Be("test@example.com");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredToken_ShouldThrowAuthenticationException()
    {
        // Arrange
        var expiredToken = "expired-refresh-token";

        // Act
        var action = () => _authService.RefreshTokenAsync(expiredToken);

        // Assert
        await action.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("Invalid refresh token");
    }
}