namespace Orders.Domain.Common;

// Domain'de olmuş bir şeyin kaydı. Adı geçmiş zamanlıdır (OrderPlaced), değişmezdir (record).
public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}
