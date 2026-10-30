using Unimag.Inventario.Api.Samples.Requests;
using Unimag.Inventario.Application.Samples.Commands.CreateSample;
using Unimag.Inventario.Application.Samples.Queries.GetSampleById;
using Unimag.Inventario.Application.Samples.Queries.ListSamples;

namespace Unimag.Inventario.Api.Samples.Mappers;

/// <summary>
/// Translates Api Requests (and route/query-string values, for reads) into
/// Application Commands/Queries. This is the only place in Api allowed to
/// construct a Command or a Query — controllers call into this mapper
/// instead of `new`-ing Application types themselves.
/// </summary>
public static class SampleMapper
{
    public static CreateSampleCommand ToCommand(this CreateSampleRequest request) =>
        new(request.Name, request.Description);

    public static GetSampleByIdQuery ToGetSampleByIdQuery(Guid id) => new(id);

    public static ListSamplesQuery ToListSamplesQuery() => new();
}
