using System.Net;
using System.Text.Json;

namespace FarmManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) { _next = next; _logger = logger; }
    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (UnauthorizedAccessException ex) { await WriteAsync(context, HttpStatusCode.Unauthorized, ex.Message); }
        catch (InvalidOperationException ex) { await WriteAsync(context, HttpStatusCode.BadRequest, ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Unhandled API error"); await WriteAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred."); }
    }
    private static async Task WriteAsync(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message, traceId = context.TraceIdentifier }));
    }
}
