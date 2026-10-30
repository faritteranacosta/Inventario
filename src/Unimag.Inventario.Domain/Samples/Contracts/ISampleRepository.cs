namespace Unimag.Inventario.Domain.Samples.Contracts;

/// <summary>
/// What the Samples domain needs from the outside world, expressed purely
/// in domain terms (it returns/receives the <see cref="Sample"/> entity,
/// never a DTO or a database row). Infrastructure provides the concrete
/// adapter (see Unimag.Inventario.Infrastructure.Samples.Repositories.SampleRepository).
/// Living in Domain means this contract has zero dependency on Application
/// or on any external package — only on the entity it describes.
/// </summary>
public interface ISampleRepository
{
    Task<Sample?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Sample>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Sample sample, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
