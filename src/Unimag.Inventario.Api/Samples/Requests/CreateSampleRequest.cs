namespace Unimag.Inventario.Api.Samples.Requests;

/// <summary>
/// HTTP-facing contract for POST /api/samples. "Request" is an Api-layer
/// concept: it belongs to this project only. It gets mapped to a
/// Unimag.Inventario.Application.Samples.Commands.CreateSample.CreateSampleCommand
/// before crossing into Application — see Samples/Mappers/SampleMapper.cs.
/// </summary>
public sealed record CreateSampleRequest(string Name, string? Description);
