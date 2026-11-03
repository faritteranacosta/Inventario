using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;
using Unimag.Inventario.Application.Purchases.Dtos;
using Unimag.Inventario.Domain.Purchases.Contracts;

namespace Unimag.Inventario.Application.Purchases.Queries.ListPurchases;

public sealed class ListPurchasesUseCase : IUseCase<ListPurchasesQuery, IReadOnlyList<PurchaseResponse>>
{
    private readonly IPurchaseRepository _repository;

    public ListPurchasesUseCase(IPurchaseRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<PurchaseResponse>> ExecuteAsync(
        ListPurchasesQuery input,
        CancellationToken cancellationToken = default)
    {
        var purchases = await _repository.GetAllAsync(cancellationToken);
        return purchases.Select(CreatePurchaseUseCase.ToResponse).ToList();
    }
}
