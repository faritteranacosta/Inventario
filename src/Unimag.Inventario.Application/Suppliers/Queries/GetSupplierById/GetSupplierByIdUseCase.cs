using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;
using Unimag.Inventario.Application.Suppliers.Dtos;
using Unimag.Inventario.Domain.Suppliers.Contracts;
using Unimag.Inventario.Domain.Suppliers.Exceptions;

namespace Unimag.Inventario.Application.Suppliers.Queries.GetSupplierById;

public sealed class GetSupplierByIdUseCase : IUseCase<GetSupplierByIdQuery, SupplierResponse>
{
    private readonly ISupplierRepository _repository;

    public GetSupplierByIdUseCase(ISupplierRepository repository) => _repository = repository;

    public async Task<SupplierResponse> ExecuteAsync(
        GetSupplierByIdQuery input,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _repository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new SupplierNotFoundException(input.Id);
        return CreateSupplierUseCase.ToResponse(supplier);
    }
}
