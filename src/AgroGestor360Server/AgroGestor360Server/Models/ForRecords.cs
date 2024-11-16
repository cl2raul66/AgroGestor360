using AgroGestor360Server.Tools.Enums;
using LiteDB;

namespace AgroGestor360Server.Models;

#region Principales
/// <summary>
/// Registro de libro diario.
/// <code>
/// Date: Fecha del registro
/// Description: Descripción del registro
/// TransactionType: Tipo de transacción
/// BankTransactions: Transacciones bancarias
/// DetailId: Id del detalle
/// </code>
/// </summary>
public class LedgerRecord
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public TransactionType TransactionType { get; set; }
    public List<BankTransaction>? BankTransactions { get; set; }
    public ObjectId? DetailId { get; set; }
}

/// <summary>
/// Registro de cuenta por pagar.
/// <code>
/// Id: Id del registro
/// InvoiceId: Id de la factura
/// SupplierId: Id del proveedor
/// AmountDue: Monto a pagar
/// DueDate: Fecha de vencimiento
/// </code>
/// </summary>
public class AccountsPayable
{
    public ObjectId? Id { get; set; }
    public string? InvoiceId { get; set; }
    public string? SupplierId { get; set; }
    public double AmountDue { get; set; }
    public DateTime DueDate { get; set; }
}

/// <summary>
/// Registro de cuenta por cobrar.
/// <code>
/// Id: Id del registro
/// InvoiceId: Id de la venta
/// AmountPaid: Monto pagado
/// DateOfPayment: Fecha del abono al crédito
/// </code>
/// </summary>
public class AccountReceivableRecord
{
    public ObjectId? Id { get; set; }
    public string? SaleReportId { get; set; }
    public double AmountPaid { get; set; }
    public DateTime DateOfPayment { get; set; }
}
#endregion
