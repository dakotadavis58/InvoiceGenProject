namespace InvoiceGeneratorAPI.Models;

/// <summary>
/// Standardized error response model for API errors
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Human-readable error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Unique identifier for the request to help with debugging
    /// </summary>
    public string TraceId { get; set; } = string.Empty;
}