﻿// Ignore Spelling: DTO

namespace AgroGestor360.Server.Models;

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
/// double [Quantity]
/// </code>
/// </summary>
public class DTO2
{
    public string? MerchandiseId { get; set; }
    public string? MerchandiseName { get; set; }
    public Presentation? Packaging { get; set; }
    public double Quantity { get; set; }
}

/// <summary>
/// Represents a article item of warehouse for PUT 
/// <code>
/// string [MerchandiseId]
/// double [Quantity]
/// </code>
/// </summary>
public class DTO2_1
{
    public string? MerchandiseId { get; set; }
    public double Quantity { get; set; }
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
/// Represents a quotation for GET
/// <code>
/// double [TotalAmount]
/// DateTime [QuotationDate]
/// string [Code, SellerId, SellerName, CustomerId, CustomerName]
/// </code>
/// </summary>
public class DTO7
{
    public double TotalAmount { get; set; }
    public DateTime QuotationDate { get; set; }
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? SellerName { get; set; }
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
}

/// <summary>
/// Represents a quotation for POST
/// <code>
/// DateTime [QuotationDate]
/// string [SellerId, CustomerId]
/// Array DTO7_3 [ProductItems]
/// </code>
/// </summary>
public class DTO7_1
{
    public DateTime QuotationDate { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public DTO7_3[]? ProductItems { get; set; }
}

/// <summary>
/// Represents a quotation for PUT
/// <code>
/// string [Code, SellerId, CustomerId]
/// Array DTO7_3 [ProductItems]
/// </code>
/// </summary>
public class DTO7_2
{
    public string? Code { get; set; }
    public string? SellerId { get; set; }
    public string? CustomerId { get; set; }
    public DTO7_3[]? ProductItems { get; set; }
}

/// <summary>
/// Object: ProductItemForQuotation
/// <code>
/// bool [HasCustomerDiscount]
/// int [OfferId]
/// double [Quantity]
/// string [ProductItems]
/// </code>
/// </summary>
public class DTO7_3
{
    public bool HasCustomerDiscount { get; set; }
    public int OfferId { get; set; }
    public double Quantity { get; set; }
    public string? ProductId { get; set; }
}
#endregion

#region Customer
/// <summary>
/// Object: Customer for GET
/// <para>
/// string [CustomerId, CustomerName], CustomerDiscountClass [Discount]
/// </para>
/// </summary>
public class DTO5_1
{
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public bool IsOrganization { get; set; }
    public CustomerDiscountClass? Discount { get; set; }
}

/// <summary>
/// Object: Customer for POST
/// <para>
/// string [CustomerFullName, CustomerAddress, CustomerPhone, CustomerMail, CustomerNIT, CustomerNIP, CustomerOccupation, CustomerOrganizationName], CustomerDiscountClass [Discount]
/// </para>
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
    public CustomerDiscountClass? Discount { get; set; }
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
    public CustomerDiscountClass? Discount { get; set; }
}

/// <summary>
/// Object: Customer for PUT by discount
/// <para>
/// string [CustomerId], int [DiscountId]
/// </para>
/// </summary>
public class DTO5_4
{
    public string? CustomerId { get; set; }
    public int DiscountId { get; set; }
}
#endregion

#region Seller
/// <summary>
/// Object: Seller for GET
/// <para>
/// string [Id, FullName]
/// </para>
/// </summary>
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
