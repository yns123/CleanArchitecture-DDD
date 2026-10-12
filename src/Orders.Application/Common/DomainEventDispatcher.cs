using Microsoft.Extensions.DependencyInjection;
using Orders.Application.Abstractions;
using Orders.Domain.Common;

namespace Orders.Application.Common;

// MediatR'ın yaptığının çok küçük hali: event'in çalışma zamanı tipine göre
// IDomainEventHandler<TEvent> dinleyicilerini DI'dan bulur ve çağırır.
public sealed class DomainEventDispatcher(IServiceProvider services) : IDomainEventDispatcher
{
    public async Task DispatchAsync(AggregateRoot aggregate, CancellationToken ct)
    {
        // Önce kopyala ve temizle: bir dinleyici aynı aggregate'e tekrar dokunursa event iki kez dağıtılmasın.
        var events = aggregate.DomainEvents.ToList();
        aggregate.ClearDomainEvents();

        foreach (var domainEvent in events)
        {
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

            foreach (var handler in services.GetServices(handlerType))
                await (Task)method.Invoke(handler, [domainEvent, ct])!;
        }
    }
}
