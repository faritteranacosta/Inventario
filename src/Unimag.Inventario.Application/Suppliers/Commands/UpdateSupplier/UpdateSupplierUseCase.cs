using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Suppliers.Commands.CreateSupplier;
using Unimag.Inventario.Application.Suppliers.Dtos;
using Unimag.Inventario.Domain.Suppliers.Contracts;
using Unimag.Inventario.Domain.Suppliers.Exceptions;

namespace Unimag.Inventario.Application.Suppliers.Commands.UpdateSupplier;

public sealed class UpdateSupplierUseCase : IUseCase<UpdateSupplierCommand, SupplierResponse>
{
    private readonly ISupplierRepository _repository;

    public UpdateSupplierUseCase(ISupplierRepository repository) => _repository = repository;

    public async Task<SupplierResponse> ExecuteAsync(
        UpdateSupplierCommand input,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _repository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new SupplierNotFoundException(input.Id);
        supplier.UpdateDetails(input.Name, input.Email, input.Phone);
        await _repository.UpdateAsync(supplier, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return CreateSupplierUseCase.ToResponse(supplier);
    }
}
