using System.Net;
using System.Text.Json;
using UnitConversionApi.Exceptions;

namespace UnitConversionApi.Middleware;

/// <summary>
/// Global exception-handling middleware that translates domain exceptions into
/// consistent RFC 7807 Problem Details JSON responses, and prevents raw stack
/// traces from leaking to clients in production.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next   = next;
        _logger = logger;
        _env    = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            UnitNotFoundException ex        => (HttpStatusCode.BadRequest,  "Unit Not Found",         ex.Message),
            IncompatibleUnitsException ex   => (HttpStatusCode.BadRequest,  "Incompatible Units",     ex.Message),
            ConversionException ex          => (HttpStatusCode.BadRequest,  "Conversion Error",       ex.Message),
            ArgumentException ex            => (HttpStatusCode.BadRequest,  "Invalid Argument",       ex.Message),
            _                               => (HttpStatusCode.InternalServerError, "Internal Server Error",
                                               _env.IsDevelopment() ? exception.Message : "An unexpected error occurred.")
        };

        _logger.LogError(exception, "Request failed: {Title} — {Detail}", title, detail);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode  = (int)statusCode;

        var problemDetails = new
        {
            type     = $"https://httpstatuses.com/{(int)statusCode}",
            title,
            status   = (int)statusCode,
            detail,
            instance = context.Request.Path.Value,
        };

        var json = JsonSerializer.Serialize(problemDetails, JsonOptions);
        await context.Response.WriteAsync(json);
    }
}

/// <summary>Extension method to register the middleware cleanly in Program.cs.</summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
