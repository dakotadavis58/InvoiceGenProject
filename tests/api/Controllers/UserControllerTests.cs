using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Moq;
using InvoiceGeneratorAPI.Controllers;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.DTOs.User;
using InvoiceGeneratorAPI.Exceptions;
using InvoiceGeneratorAPI.Common;
using FluentAssertions;
using Xunit;

namespace InvoiceGeneratorAPI.Tests.Controllers;

public class UserControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UserController _controller;
    private readonly Guid _userId;

    public UserControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UserController(_userServiceMock.Object);
        _userId = Guid.NewGuid();
    }

    private void SetupUser(string role = Roles.User, Guid? userId = null)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, (userId ?? _userId).ToString()),
            new(ClaimTypes.Role, role)
        }, "test"));

        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _controller.ControllerContext = context;
    }

    [Fact]
    public async Task GetUsers_AsAdmin_ReturnsAllUsers()
    {
        // Arrange
        SetupUser(Roles.Admin);
        var users = new List<UserDto>
        {
            new() { Id = Guid.NewGuid(), Email = "user1@example.com" },
            new() { Id = Guid.NewGuid(), Email = "user2@example.com" }
        };

        _userServiceMock.Setup(x => x.GetUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetUsers();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedUsers = okResult!.Value as List<UserDto>;
        returnedUsers.Should().NotBeNull();
        returnedUsers!.Count.Should().Be(2);
    }

    [Fact]
    public async Task GetUser_AsAdmin_ReturnsAnyUser()
    {
        // Arrange
        SetupUser(Roles.Admin);
        var targetUserId = Guid.NewGuid();
        var user = new UserDto
        {
            Id = targetUserId,
            Email = "user@example.com"
        };

        _userServiceMock.Setup(x => x.GetUserAsync(targetUserId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(targetUserId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedUser = okResult!.Value as UserDto;
        returnedUser.Should().NotBeNull();
        returnedUser!.Id.Should().Be(targetUserId);
    }

    [Fact]
    public async Task GetUser_AsUser_CanAccessOwnProfile()
    {
        // Arrange
        SetupUser(Roles.User, _userId);
        var user = new UserDto
        {
            Id = _userId,
            Email = "user@example.com"
        };

        _userServiceMock.Setup(x => x.GetUserAsync(_userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(_userId);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedUser = okResult!.Value as UserDto;
        returnedUser.Should().NotBeNull();
        returnedUser!.Id.Should().Be(_userId);
    }

    [Fact]
    public async Task GetUser_AsUser_CannotAccessOtherProfile()
    {
        // Arrange
        SetupUser(Roles.User, _userId);
        var otherUserId = Guid.NewGuid();

        // Act
        var result = await _controller.GetUser(otherUserId);

        // Assert
        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task UpdateUser_AsAdmin_CanUpdateAnyUser()
    {
        // Arrange
        SetupUser(Roles.Admin);
        var targetUserId = Guid.NewGuid();
        var request = new UpdateUserRequest
        {
            FirstName = "Updated"
        };

        var updatedUser = new UserDto
        {
            Id = targetUserId,
            FirstName = "Updated"
        };

        _userServiceMock.Setup(x => x.UpdateUserAsync(targetUserId, request))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(targetUserId, request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedUser = okResult!.Value as UserDto;
        returnedUser.Should().NotBeNull();
        returnedUser!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateUser_AsUser_CanUpdateOwnProfile()
    {
        // Arrange
        SetupUser(Roles.User, _userId);
        var request = new UpdateUserRequest
        {
            FirstName = "Updated"
        };

        var updatedUser = new UserDto
        {
            Id = _userId,
            FirstName = "Updated"
        };

        _userServiceMock.Setup(x => x.UpdateUserAsync(_userId, request))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(_userId, request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedUser = okResult!.Value as UserDto;
        returnedUser.Should().NotBeNull();
        returnedUser!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task UpdateUser_AsUser_CannotUpdateOtherProfile()
    {
        // Arrange
        SetupUser(Roles.User, _userId);
        var otherUserId = Guid.NewGuid();
        var request = new UpdateUserRequest
        {
            FirstName = "Updated"
        };

        // Act
        var result = await _controller.UpdateUser(otherUserId, request);

        // Assert
        result.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task DeleteUser_AsAdmin_CanDeleteUser()
    {
        // Arrange
        SetupUser(Roles.Admin);
        var targetUserId = Guid.NewGuid();

        _userServiceMock.Setup(x => x.DeleteUserAsync(targetUserId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(targetUserId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteUser_AsUser_ReturnsForbidden()
    {
        // Arrange
        SetupUser(Roles.User);
        var targetUserId = Guid.NewGuid();

        // Act
        var result = await _controller.DeleteUser(targetUserId);

        // Assert
        result.Should().BeOfType<ForbidResult>();
    }
}