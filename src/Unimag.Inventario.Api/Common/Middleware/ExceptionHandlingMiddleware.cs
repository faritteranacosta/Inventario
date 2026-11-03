using Unimag.Inventario.Api.Common.Errors;
using Unimag.Inventario.Domain.Categories.Exceptions;
using Unimag.Inventario.Domain.Common;
using Unimag.Inventario.Domain.Products.Exceptions;
using Unimag.Inventario.Domain.Sales.Exceptions;
using Unimag.Inventario.Domain.Tenants;
using Unimag.Inventario.Domain.Tenants.Exceptions;
using Unimag.Inventario.Domain.Users;
using Unimag.Inventario.Domain.InventoryMovements.Exceptions;
using Unimag.Inventario.Domain.Suppliers.Exceptions;
using Unimag.Inventario.Domain.Purchases.Exceptions;

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

            await context.Response.WriteAsJsonAsync(
                ErrorResponse.From(
                    GetErrorCode(statusCode, exception),
                    message,
                    null,
                    context.TraceIdentifier));
        }
    }

    /// <summary>
    /// One case per exception type that needs its own status code. New
    /// exception = add one line here, in front of the DomainException
    /// fallback if it needs something other than 400.
    /// </summary>
    private static int MapToStatusCode(Exception exception) => exception switch
    {
        CategoryNotFoundException => StatusCodes.Status404NotFound,
        ProductNotFoundException => StatusCodes.Status404NotFound,
        SaleNotFoundException => StatusCodes.Status404NotFound,
        TenantNotFoundException => StatusCodes.Status404NotFound,
        UserNotFoundException => StatusCodes.Status404NotFound,
        UserAlreadyExistsException => StatusCodes.Status409Conflict,
        InvalidCredentialsException => StatusCodes.Status401Unauthorized,
        TenantAccessDeniedException => StatusCodes.Status403Forbidden,
        TenantSuspendedException => StatusCodes.Status403Forbidden,
        InsufficientStockException => StatusCodes.Status409Conflict,
        SupplierNotFoundException => StatusCodes.Status404NotFound,
        PurchaseNotFoundException => StatusCodes.Status404NotFound,
        DomainException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    private static string GetErrorCode(int statusCode, Exception exception) => exception switch
    {
        InsufficientStockException => "INSUFFICIENT_STOCK",
        InventoryStockNegativeException => "INSUFFICIENT_STOCK",
        BarcodeAlreadyExistsException => "BARCODE_ALREADY_EXISTS",
        _ when statusCode == StatusCodes.Status400BadRequest => "INVALID_REQUEST",
        _ when statusCode == StatusCodes.Status401Unauthorized => "UNAUTHENTICATED",
        _ when statusCode == StatusCodes.Status403Forbidden => "FORBIDDEN",
        _ when statusCode == StatusCodes.Status404NotFound => "RESOURCE_NOT_FOUND",
        _ when statusCode == StatusCodes.Status409Conflict => "CONFLICT",
        _ when statusCode == StatusCodes.Status422UnprocessableEntity => "VALIDATION_ERROR",
        _ => "INTERNAL_ERROR"
    };
}
