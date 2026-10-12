using Orders.Domain.Common;
using Orders.Domain.Orders;

namespace Orders.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Same_amount_and_currency_are_equal()
        => Assert.Equal(new Money(10, "try"), new Money(10, "TRY"));

    [Fact]
    public void Negative_amount_is_rejected()
        => Assert.Throws<DomainException>(() => new Money(-1, "TRY"));

    [Fact]
    public void Adding_different_currencies_is_rejected()
        => Assert.Throws<DomainException>(() => new Money(1, "TRY").Add(new Money(1, "USD")));
}
