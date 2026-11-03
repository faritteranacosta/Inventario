using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;
using Unimag.Inventario.Application.Suppliers.Dtos;
using Unimag.Inventario.Domain.Suppliers.Contracts;

namespace Unimag.Inventario.Application.Suppliers.Queries.ListSuppliers;

public sealed class ListSuppliersUseCase : IUseCase<ListSuppliersQuery, IReadOnlyList<SupplierResponse>>
{
    private readonly ISupplierRepository _repository;

    public ListSuppliersUseCase(ISupplierRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<SupplierResponse>> ExecuteAsync(
        ListSuppliersQuery input,
        CancellationToken cancellationToken = default)
    {
        var suppliers = await _repository.GetAllAsync(cancellationToken);
        return suppliers.Select(CreateSupplierUseCase.ToResponse).ToList();
    }
}
