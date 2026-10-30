using Microsoft.AspNetCore.Mvc;
using Unimag.Inventario.Api.Common.Errors;
using Unimag.Inventario.Api.Samples.Mappers;
using Unimag.Inventario.Api.Samples.Requests;
using Unimag.Inventario.Application.Common.Interfaces;
using Unimag.Inventario.Application.Samples.Commands.CreateSample;
using Unimag.Inventario.Application.Samples.Dtos;
using Unimag.Inventario.Application.Samples.Queries.GetSampleById;
using Unimag.Inventario.Application.Samples.Queries.ListSamples;

namespace Unimag.Inventario.Api.Samples;

/// <summary>
/// Driving adapter: maps Requests to Commands/Queries via SampleMapper and
/// calls the matching Application use case. Contains no business logic and
/// no error-mapping logic — a failed use case just throws, and
/// ExceptionHandlingMiddleware turns that into the right HTTP response.
/// </summary>
[ApiController]
[Route("api/samples")]
public sealed class SamplesController : ControllerBase
{
    private readonly IUseCase<CreateSampleCommand, SampleResponse> _createSample;
    private readonly IUseCase<GetSampleByIdQuery, SampleResponse> _getSampleById;
    private readonly IUseCase<ListSamplesQuery, IReadOnlyList<SampleResponse>> _listSamples;

    public SamplesController(
        IUseCase<CreateSampleCommand, SampleResponse> createSample,
        IUseCase<GetSampleByIdQuery, SampleResponse> getSampleById,
        IUseCase<ListSamplesQuery, IReadOnlyList<SampleResponse>> listSamples)
    {
        _createSample = createSample;
        _getSampleById = getSampleById;
        _listSamples = listSamples;
    }

    [HttpPost]
    [ProducesResponseType(typeof(SampleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SampleResponse>> Create(
        [FromBody] CreateSampleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _createSample.ExecuteAsync(request.ToCommand(), cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SampleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SampleResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var response = await _getSampleById.ExecuteAsync(SampleMapper.ToGetSampleByIdQuery(id), cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SampleResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SampleResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var response = await _listSamples.ExecuteAsync(SampleMapper.ToListSamplesQuery(), cancellationToken);

        return Ok(response);
    }
}
