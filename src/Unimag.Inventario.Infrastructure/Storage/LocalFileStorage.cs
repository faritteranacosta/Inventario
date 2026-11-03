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

    private const long MaxSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public async Task<string> SaveAsync(Stream content, string fileName, CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(fileName);
        if (!AllowedExtensions.Contains(ext))
            throw new InvalidOperationException($"File type '{ext}' is not allowed.");

        if (content.CanSeek && content.Length > MaxSizeBytes)
            throw new InvalidOperationException($"File exceeds maximum size of {MaxSizeBytes / (1024 * 1024)} MB.");

        var safeName = $"{Guid.NewGuid()}{ext}";
        var fullPath = Path.Combine(_rootPath, safeName);
        await using var fs = File.Create(fullPath);
        await content.CopyToAsync(fs, cancellationToken);

        if (new FileInfo(fullPath).Length > MaxSizeBytes)
        {
            File.Delete(fullPath);
            throw new InvalidOperationException($"File exceeds maximum size of {MaxSizeBytes / (1024 * 1024)} MB.");
        }

        return $"{_baseUrl}/{safeName}";
    }
}