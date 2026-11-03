namespace Unimag.Inventario.Api.Common.Errors;

/// <summary>
/// Uniform error body for this Api.
/// </summary>
public sealed record ErrorResponse(ErrorBody Error)
{
    public static ErrorResponse From(string code, string message, object? details, string traceId) =>
        new(new ErrorBody(code, message, details, traceId));
}

public sealed record ErrorBody(string Code, string Message, object? Details, string TraceId);
