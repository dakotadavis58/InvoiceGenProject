using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.DTOs.User;
using InvoiceGeneratorAPI.Exceptions;
using FluentAssertions;
using Xunit;

namespace InvoiceGeneratorAPI.Tests.Services;

public class UserServiceTests : TestBase
{
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(Context);
    }

    [Fact]
    public async Task GetUserAsync_WithValidId_ShouldReturnUser()
    {
        // Arrange
        var user = await CreateTestUser();

        // Act
        var result = await _userService.GetUserAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
        result.FirstName.Should().Be(user.FirstName);
    }

    [Fact]
    public async Task GetUserAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var result = await _userService.GetUserAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUser_WithValidData_UpdatesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var request = new UpdateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com"
        };

        // Act
        var result = await _userService.UpdateUserAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(request.FirstName);
        result.LastName.Should().Be(request.LastName);
        result.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task UpdateUserAsync_WithInvalidId_ShouldThrowNotFoundException()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var updateRequest = new UpdateUserRequest
        {
            FirstName = "Updated"
        };

        // Act
        var action = () => _userService.UpdateUserAsync(invalidId, updateRequest);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ShouldDeleteUser()
    {
        // Arrange
        var user = await CreateTestUser();

        // Act
        var result = await _userService.DeleteUserAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        var deletedUser = await _userService.GetUserAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    private async Task<User> CreateTestUser()
    {
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
        return user;
    }
}