using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InvoiceGeneratorAPI.Services;
using InvoiceGeneratorAPI.DTOs.User;
using InvoiceGeneratorAPI.Exceptions;
using InvoiceGeneratorAPI.Common;
using InvoiceGeneratorAPI.Extensions;  // Add this for GetUserId extension
using Microsoft.Extensions.Logging;

namespace InvoiceGeneratorAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]  // Base authorization for all endpoints
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // Get current user's profile
    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            _logger.LogInformation("GetCurrentUser endpoint called");

            var currentUserId = User.GetUserId();
            _logger.LogInformation("Extracted user ID from token: {UserId}", currentUserId);

            var user = await _userService.GetUserAsync(currentUserId);
            _logger.LogInformation("User service returned: {UserFound}", user != null);

            if (user == null)
            {
                _logger.LogWarning("User not found for ID: {UserId}", currentUserId);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved user profile for ID: {UserId}", currentUserId);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile");
            throw;
        }
    }

    // Admin only - Get all users
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    // Admin only - Get specific user by ID
    [HttpGet("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _userService.GetUserAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    // Admin or self only
    [HttpPatch("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, UpdateUserRequest request)
    {
        var currentUserId = User.GetUserId();
        if (currentUserId != id && !User.IsInRole(Roles.Admin))
        {
            return Forbid();
        }

        try
        {
            var user = await _userService.UpdateUserAsync(id, request);
            return Ok(user);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    // Admin only
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (!User.IsInRole(Roles.Admin))
        {
            return Forbid();
        }

        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}