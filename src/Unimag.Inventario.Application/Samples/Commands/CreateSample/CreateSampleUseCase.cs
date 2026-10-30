using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Samples.Dtos;
using Unimag.Inventario.Domain.Samples;
using Unimag.Inventario.Domain.Samples.Contracts;

namespace Unimag.Inventario.Application.Samples.Commands.CreateSample;

/// <summary>
/// One class per use case: orchestrates the Domain and the outbound
/// contract, contains zero infrastructure/framework concerns. If
/// Sample.Create violates an invariant it throws SampleNameRequiredException
/// — this use case does not catch it, it just lets it propagate up to
/// ExceptionHandlingMiddleware.
/// </summary>
public sealed class CreateSampleUseCase : IUseCase<CreateSampleCommand, SampleResponse>
{
    private readonly ISampleRepository _sampleRepository;

    public CreateSampleUseCase(ISampleRepository sampleRepository)
    {
        _sampleRepository = sampleRepository;
    }

    public async Task<SampleResponse> ExecuteAsync(
        CreateSampleCommand command,
        CancellationToken cancellationToken = default)
    {
        var sample = Sample.Create(command.Name, command.Description);

        await _sampleRepository.AddAsync(sample, cancellationToken);
        await _sampleRepository.SaveChangesAsync(cancellationToken);

        return new SampleResponse(sample.Id, sample.Name, sample.Description, sample.CreatedAtUtc);
    }
}
