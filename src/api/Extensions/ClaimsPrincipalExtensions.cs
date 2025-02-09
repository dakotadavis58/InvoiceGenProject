using System.Security.Claims;

namespace InvoiceGeneratorAPI.Extensions;

/// <summary>
/// Extension methods for ClaimsPrincipal to help with user authentication and authorization
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the current user's ID from their claims
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal object (typically User property in controllers)</param>
    /// <returns>The user's ID as Guid, or Guid.Empty if not found</returns>
    /// <example>
    /// Usage in controller:
    /// <code>
    /// public class UserController : ControllerBase
    /// {
    ///     [HttpGet("profile")]
    ///     public IActionResult GetProfile()
    ///     {
    ///         var userId = User.GetUserId();
    ///         // Use userId to fetch user data
    ///     }
    /// }
    /// </code>
    /// </example>
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
    }
}