namespace Unimag.Inventario.Api.Common.Middleware;

public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>Register first in the pipeline so it wraps every other middleware/controller.</summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
