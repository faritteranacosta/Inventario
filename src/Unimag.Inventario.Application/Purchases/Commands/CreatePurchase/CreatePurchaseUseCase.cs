using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Purchases.Dtos;
using Unimag.Inventario.Domain.Purchases;
using Unimag.Inventario.Domain.Purchases.Contracts;

namespace Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;

public sealed class CreatePurchaseUseCase : IUseCase<CreatePurchaseCommand, PurchaseResponse>
{
    private readonly IPurchaseRepository _repository;

    public CreatePurchaseUseCase(IPurchaseRepository repository) => _repository = repository;

    public async Task<PurchaseResponse> ExecuteAsync(
        CreatePurchaseCommand input,
        CancellationToken cancellationToken = default)
    {
        var items = input.Items.Select(item =>
            PurchaseItem.Create(item.ProductId, item.Quantity, item.UnitCost));
        var purchase = Purchase.Create(input.TenantId, input.SupplierId, input.CreatedByUserId, items);
        await _repository.AddAsync(purchase, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return ToResponse(purchase);
    }

    internal static PurchaseResponse ToResponse(Purchase purchase) =>
        new(purchase.Id, purchase.SupplierId, purchase.Status, purchase.Total,
            purchase.ReceivedAtUtc, purchase.CreatedByUserId, purchase.CreatedAtUtc,
            purchase.Items.Select(item =>
                new PurchaseItemResponse(item.ProductId, item.Quantity, item.UnitCost, item.Total)).ToList());
}
