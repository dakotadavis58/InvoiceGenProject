using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using InvoiceGeneratorAPI.Controllers;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.Auth;
using InvoiceGeneratorAPI.DTOs.User;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.Exceptions;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Options;
using InvoiceGeneratorAPI.Settings;

namespace InvoiceGeneratorAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthController _controller;
    private readonly DefaultHttpContext _httpContext;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _configurationMock = new Mock<IConfiguration>();
        var googleAuthSettings = Options.Create(new GoogleAuthSettings
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        });

        _controller = new AuthController(
            _authServiceMock.Object,
            _configurationMock.Object,
            googleAuthSettings);

        // Setup HttpContext with mock cookies
        _httpContext = new DefaultHttpContext();
        var cookiesMock = new Mock<IRequestCookieCollection>();
        cookiesMock.Setup(c => c["refreshToken"]).Returns("old-refresh-token");
        _httpContext.Request.Cookies = cookiesMock.Object;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        _authServiceMock.Setup(x => x.RegisterUserAsync(request))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var createdResult = result.Result as CreatedResult;
        createdResult.Should().NotBeNull();

        var userDto = createdResult!.Value as UserDto;
        userDto.Should().NotBeNull();
        userDto!.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = "Test",
            LastName = "User"
        };

        _authServiceMock.Setup(x => x.LoginAsync(request))
            .ReturnsAsync((accessToken: "test-token", refreshToken: "refresh-token", user: userDto));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as AuthResponse;
        response.Should().NotBeNull();
        response!.Token.Should().Be("test-token");
        response.User.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsOkResult()
    {
        // Arrange
        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com"
        };

        _authServiceMock.Setup(x => x.RefreshTokenAsync("old-refresh-token"))
            .ReturnsAsync(("new-token", "new-refresh-token", userDto));

        // Act
        var result = await _controller.RefreshToken();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult!.Value as AuthResponse;
        response.Should().NotBeNull();
        response!.Token.Should().Be("new-token");
    }

    [Fact]
    public async Task RefreshToken_WithNoToken_ReturnsUnauthorized()
    {
        // Arrange
        var emptyContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = emptyContext
        };

        // Act
        var result = await _controller.RefreshToken();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Logout_ShouldRevokeTokenAndClearCookie()
    {
        // Arrange
        var existingRefreshToken = "existing-refresh-token";
        _httpContext.Request.Headers.Cookie = $"refreshToken={existingRefreshToken}";

        // Act
        var result = await _controller.Logout();

        // Assert
        var okResult = result as OkResult;
        okResult.Should().NotBeNull();

        // Verify refresh token was revoked
        _authServiceMock.Verify(x => x.RevokeRefreshTokenAsync(existingRefreshToken), Times.Once);
    }

    [Fact]
    public async Task IsEmailAvailable_WithAvailableEmail_ReturnsTrue()
    {
        // Arrange
        var email = "available@example.com";
        _authServiceMock.Setup(x => x.IsEmailAvailableAsync(email))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.IsEmailAvailable(email);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(true);
    }
}