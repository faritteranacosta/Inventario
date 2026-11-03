namespace Unimag.Inventario.Application.Common.Interfaces;

public interface IRequestMetadata
{
    string? IpAddress { get; }
    string? UserAgent { get; }
    string? Metadata { get; }
}
