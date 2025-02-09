using InvoiceGeneratorAPI.DTOs.User;

namespace InvoiceGeneratorAPI.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}
