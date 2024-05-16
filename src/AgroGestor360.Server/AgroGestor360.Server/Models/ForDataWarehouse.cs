using AgroGestor360.Server.Tools.Enums;

namespace AgroGestor360.Server.Models;

public class QuotationWarehouse
{
    public QuotationStatus Status { get; set; }
    public bool HasCustomerDiscount { get; set; }
    public int OfferId { get; set; }
    public double Quantity { get; set; }
    public ProductItemForSale? Product { get; set; }
}
