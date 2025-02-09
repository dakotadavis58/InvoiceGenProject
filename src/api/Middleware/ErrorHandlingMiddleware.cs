using System.Net;
using System.Text.Json;
using InvoiceGeneratorAPI.Exceptions;
using InvoiceGeneratorAPI.Models;

namespace InvoiceGeneratorAPI.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new ErrorResponse
            {
                TraceId = context.TraceIdentifier
            };

            switch (error)
            {
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = error.Message;
                    break;
                case DuplicateEmailException:
                    response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.Message = error.Message;
                    break;
                case AuthenticationException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse.Message = error.Message;
                    break;
                case ValidationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = error.Message;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An unexpected error occurred.";
                    _logger.LogError(error, "Unhandled exception");
                    break;
            }

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}