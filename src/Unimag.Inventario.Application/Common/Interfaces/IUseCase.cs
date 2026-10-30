namespace Unimag.Inventario.Application.Common.Interfaces;

/// <summary>
/// One use case = one class = one business operation, whether it reads
/// (backed by a Query) or writes (backed by a Command). This is a
/// deliberate middle ground, not full CQRS: a single execution abstraction
/// keeps debugging simple (one interface, one call path to step through),
/// while Commands and Queries still live in separate folders under
/// Application/{Dominio}/ for readability and clear separation of
/// responsibilities.
/// </summary>
public interface IUseCase<in TIn, TOut>
{
    Task<TOut> ExecuteAsync(TIn input, CancellationToken cancellationToken = default);
}
