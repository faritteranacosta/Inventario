using Unimag.Inventario.Api.Common.Errors;
using Unimag.Inventario.Domain.Categories.Exceptions;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Sales.Exceptions;
using Unimag.Inventario.Domain.Samples.Exceptions;

namespace Unimag.Inventario.Api.Common.Middleware;

/// <summary>
/// Catches any exception thrown from anywhere in the request pipeline
/// (Domain, Application, Infrastructure or Api), maps its concrete type to
/// an HTTP status code, and sends its message back as
/// <see cref="ErrorResponse"/>. This is the only place in the solution that
/// maps an exception to a status code — use cases don't catch these, they
/// just let them propagate.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            var statusCode = MapToStatusCode(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            // Never leak internal exception details for a 500: the client
            // gets a safe generic message, the full exception is logged above.
            var message = statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;

            await context.Response.WriteAsJsonAsync(new ErrorResponse(message));
        }
    }

    /// <summary>
    /// One case per exception type that needs its own status code. New
    /// exception = add one line here, in front of the DomainException
    /// fallback if it needs something other than 400.
    /// </summary>
    private static int MapToStatusCode(Exception exception) => exception switch
    {
        SampleNotFoundException => StatusCodes.Status404NotFound,
        CategoryNotFoundException => StatusCodes.Status404NotFound,
        ProductNotFoundException => StatusCodes.Status404NotFound,
        SaleNotFoundException => StatusCodes.Status404NotFound,
        InsufficientStockException => StatusCodes.Status409Conflict,
        DomainException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };
}
