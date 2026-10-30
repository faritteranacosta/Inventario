using System.IO;
using Microsoft.Extensions.Configuration;
using Unimag.Inventario.Application.Common.Interfaces;

namespace Unimag.Inventario.Infrastructure.Storage;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _rootPath;
    private readonly string _baseUrl;

    public LocalFileStorage(IConfiguration config)
    {
        _rootPath = config["Storage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(_rootPath);
        _baseUrl = config["Storage:BaseUrl"] ?? "/uploads";
    }

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        var safeName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var fullPath = Path.Combine(_rootPath, safeName);
        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, cancellationToken);
        return $"{_baseUrl}/{safeName}";
    }
}