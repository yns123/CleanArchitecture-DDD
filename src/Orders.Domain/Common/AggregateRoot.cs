namespace Orders.Domain.Common;

// Aggregate root'lar olayları burada biriktirir. Dağıtmak (dispatch) onların işi değil,
// Application katmanının işi: aggregate kimin dinlediğini bilmez.
public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
