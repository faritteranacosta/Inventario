namespace Unimag.Inventario.Application.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(Guid TenantId, string Name, string? Description);