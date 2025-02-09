using InvoiceGeneratorAPI.Models;
using InvoiceGeneratorAPI.DTOs.User;

namespace InvoiceGeneratorAPI.Services;

public interface IUserService
{
    Task<List<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserAsync(Guid id);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<bool> DeleteUserAsync(Guid id);
}