using Unimag.Inventario.Domain.Common;

namespace Unimag.Inventario.Domain.Tenants;

public sealed class PlatformAuditEvent : BaseEntity
{
    public Guid ActorUserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string ResourceType { get; private set; } = string.Empty;
    public Guid? ResourceId { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private PlatformAuditEvent() { }

    private PlatformAuditEvent(Guid id, Guid actorUserId, string action, string resourceType, Guid? resourceId)
    {
        Id = id;
        ActorUserId = actorUserId;
        Action = action;
        ResourceType = resourceType;
        ResourceId = resourceId;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static PlatformAuditEvent Create(Guid actorUserId, string action, string resourceType, Guid? resourceId)
        => new(Guid.NewGuid(), actorUserId, action, resourceType, resourceId);
}
