using AgroGestor360.Server.Tools.Enums;
using LiteDB;

namespace AgroGestor360.Server.Models;

#region Principales
public class LedgerRecord
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public TransactionType TransactionType { get; set; }
    public List<BankTransaction>? BankTransactions { get; set; }
    public ObjectId? DetailId { get; set; }
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
