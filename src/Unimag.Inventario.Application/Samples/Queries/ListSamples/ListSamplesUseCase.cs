using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Samples.Dtos;
using Unimag.Inventario.Domain.Samples.Contracts;

namespace Unimag.Inventario.Application.Samples.Queries.ListSamples;

public sealed class ListSamplesUseCase : IUseCase<ListSamplesQuery, IReadOnlyList<SampleResponse>>
{
    private readonly ISampleRepository _sampleRepository;

    public ListSamplesUseCase(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public async Task<IReadOnlyList<SampleResponse>> ExecuteAsync(
        ListSamplesQuery query,
        CancellationToken cancellationToken = default)
    {
        var samples = await _sampleRepository.GetAllAsync(cancellationToken);

        return samples
            .Select(s => new SampleResponse(s.Id, s.Name, s.Description, s.CreatedAtUtc))
            .ToList();
    }
}
