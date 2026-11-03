using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;
using Unimag.Inventario.Application.Purchases.Dtos;
using Unimag.Inventario.Domain.Purchases.Contracts;
using Unimag.Inventario.Domain.Purchases.Exceptions;

namespace Unimag.Inventario.Application.Purchases.Queries.GetPurchaseById;

public sealed class GetPurchaseByIdUseCase : IUseCase<GetPurchaseByIdQuery, PurchaseResponse>
{
    private readonly IPurchaseRepository _repository;

    public GetPurchaseByIdUseCase(IPurchaseRepository repository) => _repository = repository;

    public async Task<PurchaseResponse> ExecuteAsync(
        GetPurchaseByIdQuery input,
        CancellationToken cancellationToken = default)
    {
        var purchase = await _repository.GetByIdAsync(input.Id, cancellationToken)
            ?? throw new PurchaseNotFoundException(input.Id);
        return CreatePurchaseUseCase.ToResponse(purchase);
    }
}
