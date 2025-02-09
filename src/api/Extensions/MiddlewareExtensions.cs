using InvoiceGeneratorAPI.Middleware;

namespace InvoiceGeneratorAPI.Extensions;

/// <summary>
/// Extension methods for IApplicationBuilder to add custom middleware to the request pipeline
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds the global error handling middleware to the request pipeline
    /// </summary>
    /// <param name="builder">The IApplicationBuilder instance</param>
    /// <returns>The IApplicationBuilder instance</returns>
    /// <example>
    /// Usage in Program.cs:
    /// <code>
    /// var app = builder.Build();
    /// app.UseErrorHandling();
    /// </code>
    /// </example>
    public static IApplicationBuilder UseErrorHandling(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}