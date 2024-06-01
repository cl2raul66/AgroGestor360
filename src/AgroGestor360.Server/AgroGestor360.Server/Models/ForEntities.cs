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
/// Object: ImmediatePayment => Represents a payment method for immediate payment.
/// <code>
/// DateTime [Date] => Date of payment
/// double [Amount] => Amount of payment
/// string [ReferenceNo] => Reference number of payment
/// ImmediatePaymentType [Type] => Type of payment
/// </code>
/// </summary>
public class ImmediatePayment
{
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public string? ReferenceNo { get; set; }
    public ImmediatePaymentType Type { get; set; }
}

/// <summary>
/// Object: CreditPayment => Represents a payment method for credit payment.
/// <code>
/// DateTime [Date] => Date of payment
/// double [Amount] => Amount of payment
/// string [ReferenceNo] => Reference number of payment
/// CreditPaymentType [Type] => Type of payment
/// </code>
/// </summary>
public class CreditPayment
{
    public DateTime Date { get; set; }
    public double Amount { get; set; }
    public string? ReferenceNo { get; set; }
    public CreditPaymentType Type { get; set; }
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
/// Object: SaleBase => Represents a base class of sale for Quotation, Order and Invoice.
/// <code>
/// DateTime [Date]
/// string [Code]
/// Seller [Seller]
/// Customer [Customer]
/// Array ProductSaleBase [Products]
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
/// Object: Quotation => Represent a quotation.
/// <code>
/// DateTime [Date]
/// string [Code]
/// Seller [Seller]
/// Customer [Customer]
/// Array ProductSaleBase [Products]
/// Status [QuotationStatus]
/// </code>
/// </summary>
public class Quotation : SaleBase
{
    public QuotationStatus Status { get; set; }
}

/// <summary>
/// Object: Order => Represent an order.
/// <code>
/// DateTime [QuotationDate]
/// string [Code]
/// Seller [Seller]
/// Customer [Customer]
/// Array ProductItemForDocument [ProductItems]
/// OrderStatus [Status]
/// </code>
/// </summary>
public class Order : SaleBase
{
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Object: Invoice => Represent an invoice for a sale.
/// <code>
/// DateTime [QuotationDate]
/// string [Code]
/// Seller [Seller]
/// Customer [Customer]
/// Array ProductItemForDocument [ProductItems]
/// InvoiceStatus [Status]
/// int [NumberOfInstallments] => Number of installments
/// string [NumberFEL]
/// ImmediatePayments: Array of immediate payments
/// CreditsPayments: Array of credit payments
/// </code>
/// </summary>
public class Invoice : SaleBase
{
    public InvoiceStatus Status { get; set; }
    public int NumberOfInstallments { get; set; }
    public string? NumberFEL { get; set; }
    public ImmediatePayment[]? ImmediatePayments { get; set; }
    public CreditPayment[]? CreditsPayments { get; set; }
}