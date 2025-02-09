using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using InvoiceGeneratorAPI.Middleware;
using InvoiceGeneratorAPI.Exceptions;
using System.Text.Json;
using InvoiceGeneratorAPI.Models;

namespace InvoiceGeneratorAPI.Tests.Middleware;

public class ErrorHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ErrorHandlingMiddleware>> _loggerMock;
    private readonly ErrorHandlingMiddleware _middleware;

    public ErrorHandlingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
        _middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw new Exception("Test exception"),
            logger: _loggerMock.Object
        );
    }

    [Fact]
    public async Task InvokeAsync_WithApiException_ReturnsCorrectStatusCode()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var exception = new ValidationException("Invalid input");

        var middleware = new ErrorHandlingMiddleware(
            next: (innerHttpContext) => throw exception,
            _loggerMock.Object
        );

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        var error = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

        Assert.Equal(400, context.Response.StatusCode);
        Assert.Equal("Invalid input", error?.Message);
    }
}