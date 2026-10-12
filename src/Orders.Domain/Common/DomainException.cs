namespace Orders.Domain.Common;

// İş kuralı ihlallerinde fırlatılır. Teknik hata değil, "domain kuralı çiğnendi" demektir.
public class DomainException(string message) : Exception(message);
