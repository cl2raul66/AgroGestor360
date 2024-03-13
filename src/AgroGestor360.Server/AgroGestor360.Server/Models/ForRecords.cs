﻿using AgroGestor360.Server.Tools.Enums;
using LiteDB;

namespace AgroGestor360.Server.Models;

public class InventoryDispatch
{
    public ObjectId? Id { get; set; }
    public DispatchItem[]? DispatchItems { get; set; }
    public string? Reason { get; set; } // Razón del despacho: Venta, Desperdicio, Caducidad
    public DateTime DispatchDate { get; set; }
}

public class DispatchItem
{
    public ObjectId? MerchandiseId { get; set; }
    public double Quantity { get; set; }
}

public class Dispatch
{
    public ObjectId? Id { get; set; }
    public ObjectId? SellerId { get; set;}
    public bool WithFEL { get; set;}
}

#region Principales
public class LedgerRecord
{
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public TransactionType TransactionType { get; set; }
    public double TransactionAmount { get; set; }
    public ObjectId? BankAccountId { get; set; }
}

public class Inventory
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
