using Unimag.Inventario.Api.Suppliers.Requests;
using Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;
using Unimag.Inventario.Application.Suppliers.Commands.DeleteSupplier;
using Unimag.Inventario.Application.Suppliers.Commands.UpdateSupplier;
using Unimag.Inventario.Application.Suppliers.Queries.GetSupplierById;

namespace Unimag.Inventario.Api.Suppliers.Mappers;

public static class SupplierMapper
{
    public static CreateSupplierCommand ToCreateCommand(Guid tenantId, CreateSupplierRequest request) =>
        new(tenantId, request.Name, request.Email, request.Phone);

    public static UpdateSupplierCommand ToUpdateCommand(Guid id, UpdateSupplierRequest request) =>
        new(id, request.Name, request.Email, request.Phone);

    public static DeleteSupplierCommand ToDeleteCommand(Guid id) => new(id);

    public static GetSupplierByIdQuery ToGetQuery(Guid id) => new(id);
}
