namespace Orders.Application.Abstractions;

// Değişiklikleri tek seferde kaydeder. Handler "ne yapılacağını" söyler, "ne zaman kaydedileceğini" de o belirler.
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
