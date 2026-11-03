using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Domain.Suppliers.Contracts;
using Unimag.Inventario.Domain.Suppliers.Exceptions;

namespace Unimag.Inventario.Application.Suppliers.Commands.DeleteSupplier;

public sealed class DeleteSupplierUseCase : IUseCase<DeleteSupplierCommand, bool>
{
    private readonly ISupplierRepository _repository;

    public DeleteSupplierUseCase(ISupplierRepository repository) => _repository = repository;

    public async Task<bool> ExecuteAsync(
        DeleteSupplierCommand input,
        CancellationToken cancellationToken = default)
    {
        var supplier = await _repository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new SupplierNotFoundException(input.Id);
        supplier.Deactivate();
        await _repository.UpdateAsync(supplier, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
