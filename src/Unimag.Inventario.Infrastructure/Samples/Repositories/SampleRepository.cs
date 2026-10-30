using Microsoft.EntityFrameworkCore;
using Unimag.Inventario.Domain.Samples;
using Unimag.Inventario.Domain.Samples.Contracts;
using Unimag.Inventario.Infrastructure.Persistence;

namespace Unimag.Inventario.Infrastructure.Samples.Repositories;

/// <summary>
/// Driven adapter: implements the Domain-defined contract using EF Core.
/// Swapping persistence technology means replacing this class only — the
/// Domain and Application layers never change.
/// </summary>
public sealed class SampleRepository : ISampleRepository
{
    private readonly AppDbContext _dbContext;

    public SampleRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Sample?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Samples.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Sample>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Samples.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AddAsync(Sample sample, CancellationToken cancellationToken = default) =>
        await _dbContext.Samples.AddAsync(sample, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
