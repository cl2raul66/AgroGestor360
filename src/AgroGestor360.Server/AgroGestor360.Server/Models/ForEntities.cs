using AgroGestor360.Server.Tools.Enums;
using LiteDB;
using vCardLib.Models;

namespace AgroGestor360.Server.Models;

public class Organization
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class ClientDevice
{
    public ObjectId? Id { get; set; }
    public DeviceInfo? Device { get; set; }
}

public class DeviceInfo
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? SO { get; set; }
}

public class User
{
    public ObjectId? Id { get; set; }
    public vCard? Contact { get; set; }
    public bool IsAuthorized { get; set; }
    public List<string>? GroupsId { get; set; }
}

public class UserGroup
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public class Seller
{
    public ObjectId? Id { get; set; }
    public vCard? Contact { get; set; }
}

public class Customer
{
    public ObjectId? Id { get; set; }
    public vCard? Contact { get; set; }
    public DiscountForCustomer? Discount { get; set; }
    public LineCredit? Credit { get; set; }
}

public class DiscountForCustomer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Discount { get; set; }
}

public class LineCreditItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Amount { get; set; }
}

public class LineCredit : LineCreditItem
{
    public int TimeLimit { get; set; }
}

public class TimeLimitForCredit
{
    public int Id { get; set; }
    public int TimeLimit { get; set; }
}

///<summary>
/// Object: MerchandiseItem 
/// <code>
/// string [Name, Category, Description]
/// ObjectId [Id]
/// Presentation [Packaging]
/// </code>
/// </summary>
public class MerchandiseItem
{
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public ObjectId? Id { get; set; }
    public Presentation? Packaging { get; set; }
}

public class Presentation
{
    public string? Measure { get; set; }
    public string? Unit { get; set; }
    public double Value { get; set; }
}

///<summary>
/// Object: ArticleItemForWarehouse 
/// <code>
/// ObjectId [MerchandiseId]
/// string [MerchandiseName]
/// double [Quantity, Reserved]
/// Presentation [Packaging]
/// </code>
/// </summary>
public class ArticleItemForWarehouse
{
    public double Quantity { get; set; }
    public double Reserved { get; set; }
    public string? MerchandiseName { get; set; }
    public ObjectId? MerchandiseId { get; set; }
    public Presentation? Packaging { get; set; }
}

///<summary>
/// Object: ArticleItemForSale 
/// <code>
/// ObjectId [MerchandiseId]
/// string [MerchandiseName]
/// double [Price]
/// Presentation [Packaging]
/// </code>
/// </summary>
public class ArticleItemForSale
{
    public ObjectId? MerchandiseId { get; set; }
    public string? MerchandiseName { get; set; }
    public Presentation? Packaging { get; set; }
    public double Price { get; set; }
}

/// <summary>
/// Object: ProductItemForSale 
/// <code>
/// ObjectId [Id, MerchandiseId]
/// string [ProductName]
/// double [ProductQuantity, ArticlePrice] 
/// Presentation [Packaging] 
/// Array ProductOffering [Offering]
/// </code>
/// </summary>
public class ProductItemForSale
{
    public ObjectId? Id { get; set; }
    public ObjectId? MerchandiseId { get; set; }
    public Presentation? Packaging { get; set; }
    public double ArticlePrice { get; set; }
    public string? ProductName { get; set; }
    public double ProductQuantity { get; set; }
    public ProductOffering[]? Offering { get; set; }
}

public class ProductOffering
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    public double BonusAmount { get; set; }
}

/// <summary>
/// Object: BankAccount => Represents a bank account for a ledger record.
/// <code>
/// string [Number, BankName, Alias]
/// FinancialInstrumentType [InstrumentType]
/// </code>
/// </summary>
public class BankAccount
{
    public string? Number { get; set; }
    public string? BankName { get; set; }
    public string? Alias { get; set; }
    public FinancialInstrumentType InstrumentType { get; set; }
}

/// <summary>
/// Object: PaymentMethod => Representa un pago en el sistema.
/// <code>
/// PaymentType [Type] => Tipo de pago
/// PaymentCondition [Condition] => Condición del pago (inmediato o a crédito)
/// DateTime [Date] => Fecha del pago
/// decimal [Amount] => Monto del pago
/// string [ReferenceNumber] => Número de referencia del pago
/// </code>
/// </summary>
public class PaymentMethod
{
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public PaymentType Type { get; set; }
    public PaymentCondition Condition { get; set; }
}

public class Loan
{
    public ObjectId? Id { get; set; }
    public DateTime Date { get; set; }
    public string? LoanNumber { get; set; }
    public ObjectId? BankAccountId { get; set; }
    public double Amount { get; set; }
    public double Interest { get; set; }
    public string? Concept { get; set; }
    public LoanType Type { get; set; }
    public double Insurance { get; set; }
    public string? MoreDetails { get; set; }
}

public class Expense
{
    public ObjectId? Id { get; set; }
}

/// <summary>
/// Object: BankTransaction => Represents a bank transaction for a ledger record.
/// <code>
/// ObjectId [BankAccountId]
/// double [TransactionAmount]
/// </code>
/// </summary>
public class BankTransaction
{
    public ObjectId? BankAccountId { get; set; }
    public double TransactionAmount { get; set; }
}

/// <summary>
/// Object: SaleBase => Representa la base común para todas las etapas de venta.
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// </code>
/// </summary>
public abstract class SaleBase
{
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public Seller? Seller { get; set; }
    public Customer? Customer { get; set; }
    public ProductSaleBase[]? Products { get; set; }
}

/// <summary>
/// Object: ProductSaleBase => Represents a product of base sale.
/// <code>
/// bool [HasCustomerDiscount]
/// int [OfferId]
/// double [Quantity]
/// productItemForSale [Product]
/// </code>
/// </summary>
public class ProductSaleBase
{
    public bool HasCustomerDiscount { get; set; }
    public int OfferId { get; set; }
    public double Quantity { get; set; }
    public ProductItemForSale? Product { get; set; }
}

/// <summary>
/// Object: Quotation => Representa una cotización (etapa de preventa).
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// Status [QuotationStatus] => Estado de la cotización
/// </code>
/// </summary>
public class Quotation : SaleBase
{
    public QuotationStatus Status { get; set; }
}

/// <summary>
/// Object: Order => Representa un pedido (etapa de preventa).
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// OrderStatus [Status] => Estado del pedido
/// </code>
/// </summary>
public class Order : SaleBase
{
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Object: SaleRecord => Representa el registro de una venta realizada.
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// SaleStatus [Status] => Estado de la venta
/// string [NumberFEL] => Número de factura (si aplica)
/// Array PaymentMethod [Payments] => Métodos de pago utilizados
/// </code>
/// </summary>
public class SaleRecord : SaleBase
{
    public SaleStatus Status { get; set; }
    public string? NumberFEL { get; set; }
    public PaymentMethod[]? PaymentMethods { get; set; }
}

/// <summary>
/// Object: WasteSaleRecord => Representa el registro de una venta de desecho.
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// SaleStatus [Status] => Estado de la venta
/// string [NumberFEL] => Número de factura (si aplica)
/// Array PaymentMethod [Payments] => Métodos de pago utilizados
/// string [Notes] => Notas adicionales
/// </code>
/// </summary>
public class WasteSaleRecord : SaleRecord
{
    public string? Notes { get; set; }
}

/// <summary>
/// Object: ConceptForDeletedSaleRecord => Represent a concept for a deleted invoice.
/// <code>
/// string [Id, Concept]
/// </code>
/// </summary>
public class ConceptForDeletedSaleRecord
{
    public int Id { get; set; }
    public string? Concept { get; set; }
}