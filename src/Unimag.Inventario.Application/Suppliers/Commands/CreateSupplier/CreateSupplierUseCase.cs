using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Suppliers.Dtos;
using Unimag.Inventario.Domain.Suppliers;
using Unimag.Inventario.Domain.Suppliers.Contracts;

namespace Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;

public sealed class CreateSupplierUseCase : IUseCase<CreateSupplierCommand, SupplierResponse>
{
    private readonly ISupplierRepository _repository;

    public CreateSupplierUseCase(ISupplierRepository repository) => _repository = repository;

    public async Task<SupplierResponse> ExecuteAsync(
        CreateSupplierCommand input,
        CancellationToken cancellationToken = default)
    {
        var supplier = Supplier.Create(input.TenantId, input.Name, input.Email, input.Phone);
        await _repository.AddAsync(supplier, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(supplier);
    }

    internal static SupplierResponse ToResponse(Supplier supplier) =>
        new(supplier.Id, supplier.Name, supplier.Email, supplier.Phone,
            supplier.IsActive, supplier.CreatedAtUtc);
}
