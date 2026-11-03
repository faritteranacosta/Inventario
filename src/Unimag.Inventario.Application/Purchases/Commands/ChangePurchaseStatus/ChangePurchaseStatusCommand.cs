using Unimag.Inventario.Domain.Purchases;

namespace Unimag.Inventario.Application.Purchases.Commands.ChangePurchaseStatus;

public sealed record ChangePurchaseStatusCommand(Guid Id, PurchaseStatus Status, Guid UserId);
