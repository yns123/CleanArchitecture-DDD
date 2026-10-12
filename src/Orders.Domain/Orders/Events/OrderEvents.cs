using Orders.Domain.Common;

namespace Orders.Domain.Orders.Events;

public sealed record OrderPlaced(Guid OrderId, Guid CustomerId, Money Total) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}

public sealed record OrderCancelled(Guid OrderId, Guid CustomerId) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
