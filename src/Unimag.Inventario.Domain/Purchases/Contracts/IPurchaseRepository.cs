using Unimag.Inventario.Domain.Purchases;

namespace Unimag.Inventario.Domain.Purchases.Contracts;

public interface IPurchaseRepository
{
    Task<Purchase?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Purchase>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Purchase> Items, int TotalItems)> GetPageAsync(
        int skip, int take, CancellationToken cancellationToken = default);
    Task AddAsync(Purchase purchase, CancellationToken cancellationToken = default);
    Task UpdateAsync(Purchase purchase, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
