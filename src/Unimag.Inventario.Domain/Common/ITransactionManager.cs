namespace Unimag.Inventario.Domain.Common;

public interface ITransactionManager
{
    Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default);
}
