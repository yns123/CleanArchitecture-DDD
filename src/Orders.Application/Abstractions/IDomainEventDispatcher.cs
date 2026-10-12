using Orders.Domain.Common;

namespace Orders.Application.Abstractions;

public interface IDomainEventDispatcher
{
    // Aggregate'in biriktirdiği event'leri ilgili dinleyicilere dağıtır ve listeyi temizler.
    Task DispatchAsync(AggregateRoot aggregate, CancellationToken ct);
}
