using Unimag.Inventario.Api.Categories.Requests;
using Unimag.Inventario.Application.Categories.Commands.CreateCategory;
using Unimag.Inventario.Application.Categories.Commands.DeleteCategory;
using Unimag.Inventario.Application.Categories.Commands.UpdateCategory;
using Unimag.Inventario.Application.Categories.Queries.GetCategoryById;
using Unimag.Inventario.Application.Categories.Queries.ListCategories;

namespace Unimag.Inventario.Api.Categories.Mappers;

public static class CategoryMapper
{
    public static CreateCategoryCommand ToCommand(Guid tenantId, CreateCategoryRequest request) =>
        new(tenantId, request.Name, request.Description);

    public static UpdateCategoryCommand ToCommand(Guid id, UpdateCategoryRequest request) =>
        new(id, request.Name, request.Description);

    public static DeleteCategoryCommand ToDeleteCommand(Guid id) =>
        new(id);

    public static GetCategoryByIdQuery ToQuery(Guid id) =>
        new(id);

    public static ListCategoriesQuery ToListQuery() =>
        new();
}