﻿// Ignore Spelling: DTO

using AgroGestor360Client.Tools;

namespace AgroGestor360Client.Models;

#region REPORTS
public record SaleReportParameters(
    string ReportState,
    string OrderBy,
    DateTime? BeginDate,
    DateTime EndDate,
    string? CustomerId,
    string? SellerId
);
#endregion

/// <summary>
/// Organization: Represents an organization.
/// <code>
/// string [Id, Name, Address, Phone, Email]
/// </code>
/// </summary>
public class Organization
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class ReconciliationPolicy
{
    public int Id { get; set; }
    public TypeFrequencyReconciliationPolicy Frequency { get; set; }
    public TimeSpan Time { get; set; }
}

/// <summary>
/// Represents a presentation.
/// </summary>
public class Presentation
{
    public string? Measure { get; set; }
    public string? Unit { get; set; }
    public double Value { get; set; }
}

/// <summary>
/// Represents a product offering.
/// </summary>
public class ProductOffering
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    public double BonusAmount { get; set; }
}

public class DiscountForCustomer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Discount { get; set; }
}

public class LineCredit : LineCreditItem
{
    public int TimeLimit { get; set; }
}

public class LineCreditItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Amount { get; set; }
}

public class TimeLimitForCredit
{
    public int Id { get; set; }
    public int TimeLimit { get; set; }
}

/// <summary>
/// Objeto: BankItem => Representa un elemento banco.
/// <code>
/// string [Id, Name]
/// </code>
/// </summary>
public class BankItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
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

#region Customer
/// <summary>
/// Object: Customer for GET
/// <code>
/// string [CustomerId, CustomerName] 
/// CustomerDiscountClass [Discount]
/// </code>
/// </summary>
public class DTO5_1
{
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public bool IsOrganization { get; set; }
    public DiscountForCustomer? Discount { get; set; }
    public LineCredit? Credit { get; set; }
}

/// <summary>
/// Object: Customer for POST
/// <code>
/// string [CustomerFullName, CustomerAddress, CustomerPhone, CustomerMail, CustomerNIT, CustomerNIP, CustomerOccupation, CustomerOrganizationName]
/// CustomerDiscountClass [Discount]
/// </code>
/// </summary>
public class DTO5_2
{
    public DateTime? Birthday { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerMail { get; set; }
    public string? CustomerNIT { get; set; }
    public string? CustomerNIP { get; set; }
    public string? CustomerOccupation { get; set; }
    public string? CustomerOrganizationName { get; set; }
    public DiscountForCustomer? Discount { get; set; }
    public LineCredit? Credit { get; set; }
}

/// <summary>
/// Object: Customer for PUT by customer and/or discount
/// <code>
/// string [CustomerFullName, CustomerAddress, CustomerPhone, CustomerMail, CustomerNIT, CustomerNIP, CustomerOccupation, CustomerOrganizationName] 
/// CustomerDiscountClass [Discount]
/// </code>
/// </summary>
public class DTO5_3
{
    public string? CustomerId { get; set; }
    public DateTime? Birthday { get; set; }
    public string? CustomerFullName { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerMail { get; set; }
    public string? CustomerNIT { get; set; }
    public string? CustomerNIP { get; set; }
    public string? CustomerOccupation { get; set; }
    public string? CustomerOrganizationName { get; set; }
    public DiscountForCustomer? Discount { get; set; }
    public LineCredit? Credit { get; set; }
}

/// <summary>
/// Object: Customer for PUT by discount
/// <code>
/// string [CustomerId], int [DiscountId]
/// </code>
/// </summary>
public class DTO5_4
{
    public string? CustomerId { get; set; }
    public int DiscountId { get; set; }
}

/// <summary>
/// Object: Customer for PUT by credit
/// <code>
/// string [CustomerId]
/// LineCredit [Credit]
/// </code>
/// </summary>
public class DTO5_5
{
    public string? CustomerId { get; set; }
    public LineCredit? Credit { get; set; }
}
#endregion

#region Seller
/// <summary>
/// Object: Seller for GET
/// <para>
/// string [Id, FullName]
/// </para>
/// </summary>
public class DTO6
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
}

/// <summary>
/// Object: Seller for POST
/// <para>
/// DateTime [Birthday], string [FullName, Address, Phone, Mail, NIT, NIP, Occupation], 
/// </para>
/// </summary>
public class DTO6_1
{
    public DateTime? Birthday { get; set; }
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Mail { get; set; }
    public string? NIT { get; set; }
    public string? NIP { get; set; }
    public string? Occupation { get; set; }
}

/// <summary>
/// Object: Seller for PUT
/// <para>
/// DateTime [Birthday], string [Id, FullName, Address, Phone, Mail, NIT, NIP, Occupation], 
/// </para>
/// </summary>
public class DTO6_2
{
    public DateTime? Birthday { get; set; }
    public string? Id { get; set; }
    public string? FullName { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Mail { get; set; }
    public string? NIT { get; set; }
    public string? NIP { get; set; }
    public string? Occupation { get; set; }
}
#endregion

#region Merchandise
/// <summary>
/// Represents a merchandise item for GET, POST and PUT operations.
/// <code>
/// string [Id, Name, Description, Category]
/// Presentation [Packaging]
/// </code>
/// </summary>
public class DTO1
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public Presentation? Packaging { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}
#endregion

#region Warehouse
/// <summary>
/// Represents a article item of warehouse for GET 
/// <code>
/// string [MerchandiseId, MerchandiseName]
/// Presentation [Packaging]
/// double [Quantity, Reserved]
/// </code>
/// </summary>
public class DTO2
{
    public string? MerchandiseId { get; set; }
    public string? MerchandiseName { get; set; }
    public Presentation? Packaging { get; set; }
    public double Quantity { get; set; }
    public double Reserved { get; set; }
}

/// <summary>
/// Represents a article item of warehouse for PUT 
/// <code>
/// string [MerchandiseId]
/// double [Quantity, Reserved]
/// </code>
/// </summary>
public class DTO2_1
{
    public string? MerchandiseId { get; set; }
    public double Quantity { get; set; }
    public double Reserved { get; set; }
}
#endregion

#region Sales
/// <summary>
/// Represents a article item of Sale for GET
/// <code>
/// string [MerchandiseId, MerchandiseName] 
/// Presentation [Packaging] 
/// double [Price] 
/// </code>
/// </summary>
public class DTO3
{
    public string? MerchandiseId { get; set; }
    public string? MerchandiseName { get; set; }
    public Presentation? Packaging { get; set; }
    public double Price { get; set; }
}

/// <summary>
/// Represents a article item of sale for PUT
/// <code>
/// string [MerchandiseId] 
/// double [Price] 
/// </code>
/// </summary>
public class DTO3_1
{
    public string? MerchandiseId { get; set; }
    public double Price { get; set; }
}

/// <summary>
/// Represents a product item of sale for GET
/// <code>
/// string [Id, MerchandiseId, ProductName]
/// double [ProductQuantity, ArticlePrice] 
/// Presentation [Packaging] 
/// bool [HasOffers]
/// </code>
/// </summary>
public class DTO4
{
    public string? Id { get; set; }
    public string? MerchandiseId { get; set; }
    public string? ProductName { get; set; }
    public double ProductQuantity { get; set; }
    public double ArticlePrice { get; set; }
    public Presentation? Packaging { get; set; }
    public bool HasOffers { get; set; }
}

/// <summary>
/// Represents a product item of sale for POST
/// <code>
/// string [Id, MerchandiseId, ProductName]
/// double [ProductQuantity, ArticlePrice] 
/// Presentation [Packaging] 
/// </code>
/// </summary>
public class DTO4_1
{
    public string? Id { get; set; }
    public string? MerchandiseId { get; set; }
    public string? ProductName { get; set; }
    public double ProductQuantity { get; set; }
    public double ArticlePrice { get; set; }
    public Presentation? Packaging { get; set; }
}

/// <summary>
/// Represents a product item of sale for PUT by quantity
/// <code>
/// string [Id]
/// double [ProductQuantity] 
/// </code>
/// </summary>
public class DTO4_2
{
    public string? Id { get; set; }
    public double ProductQuantity { get; set; }
}

/// <summary>
/// Represents a product item of sale for PUT by adding an offering
/// <code>
/// string [Id]
/// ProductOffering [Offer] 
/// </code>
/// </summary>
public class DTO4_3
{
    public string? Id { get; set; }
    public ProductOffering? Offer { get; set; }
}

/// <summary>
/// Represents a product item of sale for PUT by removing an offering
/// <code>
/// string [Id]
/// int [OfferId] 
/// </code>
/// </summary>
public class DTO4_4
{
    public string? Id { get; set; }
    public int OfferId { get; set; }
}
//todo: esto es para mostrar la información del producto para eliminar
/// <summary>
/// Represents a product item of sale for PUT by removing an offering
/// <code>
/// string [ProductName]
/// double [ProductQuantity, ArticlePrice] 
/// </code>
/// </summary>
public class DTO4_5
{
    public string? ProductName { get; set; }
    public double ProductQuantity { get; set; }
    public double ArticlePrice { get; set; }
    public Presentation? Packaging { get; set; }
    public ProductOffering? Offer { get; set; }
}

/// <summary>
/// Represents a quotation for GET or POST from order
/// <code>
/// bool [IsDraftStatus]
/// double [TotalAmount]
/// DateTime [QuotationDate]
/// string [Code, SellerId, SellerName, CustomerId, CustomerName]
/// </code>
/// </summary>
public class DTO7
{
    public bool IsDraftStatus { get; set; }
    public double TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? SellerName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
}

/// <summary>
/// Represents a quotation for POST
/// <code>
/// QuotationStatus [Status]
/// DateTime [QuotationDate]
/// string [SellerId, CustomerId]
/// Array DTO9 [ProductsItemsForSale]
/// </code>
/// </summary>
public class DTO7_1
{
    public QuotationStatus Status { get; set; }
    public DateTime Date { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public DTO9[]? ProductItems { get; set; }
}

/// <summary>
/// Represents a quotation for PUT
/// <code>
/// QuotationStatus [Status]
/// string [Code, SellerId, CustomerId]
/// Array DTO9 [ProductsItemsForSale]
/// </code>
/// </summary>
public class DTO7_2
{
    public QuotationStatus Status { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public DTO9[]? ProductItems { get; set; }
}

/// <summary>
/// Represents a quotation for PUT or Delete by status
/// <code>
/// QuotationStatus [Status]
/// string [Code]
/// </code>
/// </summary>
public class DTO7_3
{
    public QuotationStatus Status { get; set; }
    public string? Code { get; set; }
}

public class DTO7_4
{
    public double TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string? SellerName { get; set; }
    public string? OrganizationName { get; set; }
    public string? CustomerName { get; set; }
    public string[]? Products { get; set; }
    public QuotationStatus Status { get; set; }
}

/// <summary>
/// Represents a order for GET
/// <code>
/// bool [IsPendingStatus]
/// double [TotalAmount]
/// DateTime [Date]
/// string [Code, SellerId, SellerName, CustomerId, CustomerName]
/// </code>
/// </summary>
public class DTO8
{
    public bool IsPendingStatus { get; set; }
    public double TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? SellerName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
}

/// <summary>
/// Represents a order for POST
/// <code>
/// OrderStatus [Status]
/// DateTime [QuotationDate]
/// string [Code, SellerId, CustomerId]
/// Array DTO9 [ProductsItemsForSale]
/// </code>
/// </summary>
public class DTO8_1
{
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public DTO9[]? ProductItems { get; set; }
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Represents a order for PUT or GET from quotation
/// <code>
/// string [Code, SellerId, CustomerId]
/// Array DTO9 [ProductsItemsForSale]
/// OrderStatus [Status]
/// </code>
/// </summary>
public class DTO8_2
{
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public DTO9[]? ProductItems { get; set; }
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Represents a order for PUT by products and status
/// <code>
/// string [Code]
/// Array DTO9 [ProductsItemsForSale]
/// OrderStatus [Status]
/// </code>
/// </summary>
public class DTO8_3
{
    public string? Code { get; set; }
    public DTO9[]? ProductItems { get; set; }
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Represents a entity for create order from a quotation or invoice from order.
/// <code>
/// string [Code]
/// DTO5_1 [Customer]
/// DTO6 [Seller]
/// Array DTO9 [Products]
/// </code>
/// </summary>
public class DTO_SB1
{
    public string? Code { get; set; }
    public DTO5_1? Customer { get; set; }
    public DTO6? Seller { get; set; }
    public DTO9[]? Products { get; set; }
}

/// <summary>
/// Object: Order for GET by code
/// </summary>
public class DTO8_5
{
    public double TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string? SellerName { get; set; }
    public string? OrganizationName { get; set; }
    public string? CustomerName { get; set; }
    public string[]? Products { get; set; }
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Object: Order for DELETE by status
/// <code>
/// string [Code] => Code of the order
/// orderStatus [Status] => Status of the order
/// </code>
/// </summary>
public class DTO8_6
{
    public string? Code { get; set; }
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Object: ProductItemForDocument
/// <code>
/// bool [HasCustomerDiscount]
/// int [OfferId]
/// double [Quantity]
/// string [ProductItemForSaleId]
/// </code>
/// </summary>
public class DTO9
{
    public bool HasCustomerDiscount { get; set; }
    public int OfferId { get; set; }
    public double Quantity { get; set; }
    public string? ProductItemForSaleId { get; set; }
}

/// <summary>
/// Represents a invoice of GET method for invoice with credit payment type.
/// <code>
/// Paid: Amount paid
/// DaysRemaining: Days remaining to pay
/// TotalAmount: Total amount of the invoice
/// Date: Date of the invoice
/// Code: Code of the invoice and key in database
/// SellerId: Id of the seller
/// SellerName: Name of the seller
/// CustomerId: Id of the customer
/// CustomerName: Name of the customer
/// NumberFEL: Number of the invoice in FEL
/// Status: Status of the invoice
/// </code>
/// </summary>
public class DTO10
{
    public double Paid { get; set; }
    public int DaysRemaining { get; set; }
    public double TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? SellerName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? NumberFEL { get; set; }
    public SaleStatus Status { get; set; }
}

/// <summary>
/// Represents a invoice for POST
/// <code>
/// Date: Date of the invoice
/// Code: Code of the invoice and key in database
/// SellerId: Id of the seller
/// CustomerId: Id of the customer
/// TimeLimit: 
/// Status: Status of the invoice
/// Products: Array of products
/// </code>
/// </summary>
public class DTO10_1
{
    public DateTime Date { get; set; }
    public TimeLimitForCredit? TimeCredit { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public SaleStatus Status { get; set; }
    public DTO9[]? Products { get; set; }
}

/// <summary>
/// Represents a invoice for normal POST and POST from quote.
/// <code>
/// Code: Code of the invoice and key in database from invoice or quote
/// PaymentMethod: Payment method
/// </code>
/// </summary>
public class DTO10_2
{
    public string? Code { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}

/// <summary>
/// Represents a invoice for UPDATE by status.
/// <code>
/// Code: Code of the invoice and key in database
/// Notes: Notes for justification by status change
/// Status: Status of the invoice
/// </code>
/// </summary>
public class DTO10_3
{
    public string? Code { get; set; }
    public string? Notes { get; set; }
    public SaleStatus Status { get; set; }
}

/// <summary>
/// Represents a invoice for GET method for a invoice viewer.
/// <code>
/// Paid: Amount paid
/// DaysRemaining: Days remaining to pay if is credit
/// TotalAmount: Total amount of the invoice
/// Date: Date of the invoice
/// Code: Code of the invoice and key in database
/// SellerName: Name of the seller
/// CustomerName: Name of the customer
/// NumberFEL: Number of the invoice in FEL
/// Status: Status of the invoice
/// Products: Array of products
/// ImmediatePayments: Array of immediate payments (no credit)
/// CreditsPayments: Array of credit payments (credit)
/// </code>
/// </summary>
public class DTO10_4
{
    public double Paid { get; set; }
    public int DaysRemaining { get; set; }
    public double TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string? Code { get; set; }
    public string? SellerName { get; set; }
    public string? OrganizationName { get; set; }
    public string? CustomerName { get; set; }
    public string? NumberFEL { get; set; }
    public SaleStatus Status { get; set; }
    public string[]? Products { get; set; }
    public PaymentMethod[]? PaymentMethods { get; set; }
}

/// <summary>
/// Represents a invoice for POST
/// <code>
/// Date: Date of the invoice
/// Code: Code of the invoice and key in database
/// SellerId: Id of the seller
/// CustomerId: Id of the customer
/// NumberFEL: Number of the invoice in FEL
/// Status: Status of the invoice
/// Products: Array of products
/// ImmediatePayments: Array of immediate payments
/// CreditsPayments: Array of credit payments
/// </code>
/// </summary>
public class DTO10_5
{
    public DateTime Date { get; set; }
    public int DaysRemaining { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public string? NumberFEL { get; set; }
    public SaleStatus Status { get; set; }
    public DTO9[]? Products { get; set; }
    public PaymentMethod[]? PaymentMethods { get; set; }
}
#endregion
