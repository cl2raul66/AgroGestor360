﻿using AgroGestor360.Server.Tools.Enums;
using LiteDB;

namespace AgroGestor360.Server.Models;

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
/// InvoiceId: Id de la factura
/// CustomerId: Id del cliente
/// DueDate: Fecha de vencimiento
/// AmountDue: Monto a cobrar
/// </code>
/// </summary>
public class AccountsReceivable
{
    public ObjectId? Id { get; set; }
    public string? InvoiceId { get; set; }
    public string? CustomerId { get; set; }
    public double AmountDue { get; set; }
    public DateTime DueDate { get; set; }
}
#endregion
