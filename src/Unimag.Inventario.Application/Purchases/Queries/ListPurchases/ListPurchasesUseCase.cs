using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Purchases.Commands.CreatePurchase;
using Unimag.Inventario.Application.Purchases.Dtos;
using Unimag.Inventario.Domain.Purchases.Contracts;
using Unimag.Inventario.Application.Common.Dtos;
using Unimag.Inventario.Domain.Common.Exceptions;

namespace Unimag.Inventario.Application.Purchases.Queries.ListPurchases;

public sealed class ListPurchasesUseCase : IUseCase<ListPurchasesQuery, PagedResponse<PurchaseResponse>>
{
    private readonly IPurchaseRepository _repository;

    public ListPurchasesUseCase(IPurchaseRepository repository) => _repository = repository;

    public async Task<PagedResponse<PurchaseResponse>> ExecuteAsync(
        ListPurchasesQuery input,
        CancellationToken cancellationToken = default)
    {
        if (input.Page < 1 || input.PageSize is < 1 or > 100)
        {
            throw new InvalidPaginationException();
        }

        var result = await _repository.GetPageAsync(
            (input.Page - 1) * input.PageSize, input.PageSize, cancellationToken);
        var items = result.Items
            .Select(CreatePurchaseUseCase.ToResponse)
            .ToList();
        return new PagedResponse<PurchaseResponse>(
            items, input.Page, input.PageSize, result.TotalItems,
            (int)Math.Ceiling(result.TotalItems / (double)input.PageSize));
    }
}
