using Orders.Domain.Common;

namespace Orders.Application.Abstractions;

// Bir event'e tepki veren dinleyici. Aynı event'i istediği kadar dinleyici dinleyebilir.
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct);
}
