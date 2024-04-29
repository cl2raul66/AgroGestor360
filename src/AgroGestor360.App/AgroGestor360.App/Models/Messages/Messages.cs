using AgroGestor360.Client.Models;

namespace AgroGestor360.App.Models;

public record PgAddWarehouseMessage(DTO1 Merchandise, double Quantity);
public record PgAddProductMessage(string ArticleId, string Name, double Quantity);
