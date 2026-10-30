using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.UnitTests.Fakes;

public sealed class FakeCurrentTenant : ICurrentTenant
{
    public Guid TenantId { get; set; } = Guid.NewGuid();
}