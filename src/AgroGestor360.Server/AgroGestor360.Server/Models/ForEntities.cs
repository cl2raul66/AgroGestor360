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
    public CustomerDiscountClass? Discount { get; set; }
}

public class CustomerDiscountClass
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Value { get; set; }
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
/// double [Quantity]
/// Presentation [Packaging]
/// </code>
/// </summary>
public class ArticleItemForWarehouse
{
    public double Quantity { get; set; }
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

public class BankAccount
{
    public string? Number { get; set; }
    public string? BankName{ get; set; }
    public string? Alias { get; set; }
    public FinancialInstrumentType InstrumentType { get; set; }
}

public class Sale
{
    public ObjectId? Id { get; set; }
    public List<SaleProduct>? SaleProducts { get; set; }
    public ObjectId? SellerId { get; set; }
    public ObjectId? CustomerId { get; set; }
    public bool IsWithFEL { get; set; }
    public List<PaymentMethod>? Payments { get; set; }
}

public abstract class PaymentMethod
{
    public double Amount { get; set; }
}

public class ImmediatePayment : PaymentMethod
{
    public string? Reference { get; set; }
    public ObjectId? BankAccountId { get; set; }
    public ImmediatePaymentType Type { get; set; }
}

public class CreditPayment : PaymentMethod
{
    public CreditPaymentType Type { get; set; }
    public int NumberOfInstallments { get; set; }
}

public class SaleProduct
{
    public ObjectId? ProductId { get; set; }
    public double Quantity { get; set; }
    public ObjectId? ProductOfferingId { get; set; }
    public ObjectId? CustomerDiscountClassId { get; set; }
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

public class BankTransaction
{
    public ObjectId? BankAccountId { get; set; }
    public double TransactionAmount { get; set; }
}

/// <summary>
/// Object: Quotation
/// <code>
/// bool [HasCustomerDiscount, WasDelivered]
/// DateTime [QuotationDate]
/// Guid [Code]
/// Seller [Seller]
/// Customer [Customer]
/// Array ProductItemQuotation [ProductItems]
/// </code>
/// </summary>
public class Quotation
{
    public bool WasDelivered { get; set; }
    public DateTime QuotationDate { get; set; }
    public Guid Code { get; set; }
    public Seller? Seller { get; set; }
    public Customer? Customer { get; set; }
    public ProductItemForQuotation[]? ProductItems { get; set; }
}

/// <summary>
/// Object: ProductItemForQuotation
/// <code>
/// int [OfferId]
/// double [Quantity]
/// productItemForSale [Product]
/// </summary>
public class ProductItemForQuotation
{
    public bool HasCustomerDiscount { get; set; }
    public int OfferId { get; set; }
    public double Quantity { get; set; }
    public ProductItemForSale? Product { get; set; }
}
