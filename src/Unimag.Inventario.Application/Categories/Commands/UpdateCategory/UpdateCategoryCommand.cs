namespace Unimag.Inventario.Application.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(Guid Id, string Name, string? Description);