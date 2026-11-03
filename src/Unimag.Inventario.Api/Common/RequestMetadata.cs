using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Unimag.Inventario.Application.Common.Interfaces;

namespace Unimag.Inventario.Api.Common;

public sealed class RequestMetadata(IHttpContextAccessor accessor) : IRequestMetadata
{
    public string? IpAddress => accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    public string? UserAgent => accessor.HttpContext?.Request.Headers.UserAgent.ToString();
    public string? Metadata => accessor.HttpContext is null
        ? null
        : JsonSerializer.Serialize(new
        {
            method = accessor.HttpContext.Request.Method,
            path = accessor.HttpContext.Request.Path.Value
        });
}
