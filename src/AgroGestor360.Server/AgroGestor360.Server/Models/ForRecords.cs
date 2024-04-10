using AgroGestor360.Server.Tools.Enums;
using LiteDB;

namespace AgroGestor360.Server.Models;

public class InventoryDispatch
{
    public ObjectId? Id { get; set; }
    public List<DispatchItem>? DispatchItems { get; set; }
    public string? Reason { get; set; }
    public DateTime DispatchDate { get; set; }
}

public class DispatchItem
{
    public ObjectId? MerchandiseId { get; set; }
    public double Quantity { get; set; }
}

#region Principales
public class LedgerRecord
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public TransactionType TransactionType { get; set; }
    public List<BankTransaction>? BankTransactions { get; set; }
    public ObjectId? DetailId { get; set; }
}

public class WarehouseItem
{
    public ObjectId? Id { get; set; }
    public ObjectId? MerchandiseId { get; set; }
    public double Quantity { get; set; }
}

public class AccountsPayable
{
    public ObjectId? Id { get; set; }
    public string? InvoiceId { get; set; }
    public string? SupplierId { get; set; }
    public double AmountDue { get; set; }
    public DateTime DueDate { get; set; }
}

public class AccountsReceivable
{
    public ObjectId? Id { get; set; }
    public string? InvoiceId { get; set; }
    public string? CustomerId { get; set; }
    public double AmountDue { get; set; }
    public DateTime DueDate { get; set; }
}
#endregion
