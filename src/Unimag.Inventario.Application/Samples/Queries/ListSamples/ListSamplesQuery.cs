namespace Unimag.Inventario.Application.Samples.Queries.ListSamples;

/// <summary>Queries with no input still take a (possibly empty) query, to keep IQueryHandler uniform.</summary>
public sealed record ListSamplesQuery;
