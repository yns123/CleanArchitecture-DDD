using Orders.Domain.Common;

namespace Orders.Domain.Orders;

// Value Object: kimliği yok, değeriyle tanımlanır. Değişmez (immutable).
// record -> değer eşitliği otomatik gelir: new Money(10,"TRY") == new Money(10,"TRY")
public sealed record Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new DomainException("Tutar negatif olamaz.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Para birimi zorunlu.");
        Amount = decimal.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency) => new(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int factor) => new(Amount * factor, Currency);

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Farklı para birimleri toplanamaz.");
    }
}
