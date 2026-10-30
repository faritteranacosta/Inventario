using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Samples.Dtos;
using Unimag.Inventario.Domain.Samples.Contracts;
using Unimag.Inventario.Domain.Samples.Exceptions;

namespace Unimag.Inventario.Application.Samples.Queries.GetSampleById;

public sealed class GetSampleByIdUseCase : IUseCase<GetSampleByIdQuery, SampleResponse>
{
    private readonly ISampleRepository _sampleRepository;

    public GetSampleByIdUseCase(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public async Task<SampleResponse> ExecuteAsync(
        GetSampleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var sample = await _sampleRepository.GetByIdAsync(query.Id, cancellationToken)
            ?? throw new SampleNotFoundException(query.Id);

        return new SampleResponse(sample.Id, sample.Name, sample.Description, sample.CreatedAtUtc);
    }
}
