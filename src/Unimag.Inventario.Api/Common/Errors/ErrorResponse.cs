namespace Unimag.Inventario.Api.Common.Errors;

/// <summary>
/// Uniform error body for this Api. Every error response — whether it comes
/// from a Result.Failure mapped inside a controller or from an exception
/// caught by ExceptionHandlingMiddleware — has this exact shape.
/// </summary>
public sealed record ErrorResponse(string Message);
