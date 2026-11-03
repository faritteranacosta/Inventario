using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Infrastructure.Persistence;

public sealed class EfTransactionManager(AppDbContext db) : ITransactionManager
{
    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        if (db.Database.IsInMemory())
            return await operation(cancellationToken);

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
