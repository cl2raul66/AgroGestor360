﻿// Ignore Spelling: DTO

using AgroGestor360.Client.Tools;

namespace AgroGestor360.Client.Models;

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

/// <summary>
/// BankAccount: Represents a bank account.
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
/// Represents a merchandise category.
/// </summary>
public class MerchandiseCategory
{
    public string? Name { get; set; }
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
    public int? Id { get; set; }
    public double Quantity { get; set; }
    public double BonusAmount { get; set; }
}

/// <summary>
/// Represents a customer discount class.
/// </summary>
public class CustomerDiscountClass
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Value { get; set; }
}

#region Merchandise
/// <summary>
/// Represents a merchandise item.
/// </summary>
public class DTO1
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public Presentation? Packaging { get; set; }
    public MerchandiseCategory? Category { get; set; }
    public string? Description { get; set; }
}
#endregion

#region Warehouse
/// <summary>
/// Represents an article item for warehouse.
/// </summary>
public class DTO2
{
    public string? Id { get; set; }
    public DTO1? Merchandise { get; set; }
    public double Quantity { get; set; }
}

/// <summary>
/// Represents an article item for warehouse.
/// </summary>
public class DTO2_1
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public double Value { get; set; }
    public string? Category { get; set; }
    public double Quantity { get; set; }
}
#endregion

#region Sales
/// <summary>
/// Represents an article item for sale.
/// </summary>
public class DTO3
{
    public string? Id { get; set; }
    public DTO1? Merchandise { get; set; }
    public double Price { get; set; }
}

/// <summary>
/// Represents an article item for sale.
/// </summary>
public class DTO3_1
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
    public double Value { get; set; }
    public string? Category { get; set; }
    public double Price { get; set; }
}

/// <summary>
/// Represents a product item for sale.
/// </summary>
public class DTO4
{
    public string? Id { get; set; }
    public DTO3? Article { get; set; }
    public string? Name { get; set; }
    public double Quantity { get; set; }
    public List<ProductOffering>? Offering { get; set; }
}

/// <summary>
/// Represents a product item for sale.
/// </summary>
public class DTO4_1
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public double Quantity { get; set; }
    public double SalePrice { get; set; }
    public string? Unit { get; set; }
    public double Value { get; set; }
    public string? Category { get; set; }
}

/// <summary>
/// Represents a product item for sale.
/// </summary>
public class DTO4_2
{
    public string? Id { get; set; }
    public double Quantity { get; set; }
}

/// <summary>
/// Represents a product item for sale.
/// </summary>
public class DTO4_3
{
    public string? Id { get; set; }
    public List<ProductOffering>? Offering { get; set; }
}
#endregion

#region Customer
/// <summary>
/// Represents a customer for GET.
/// <code>
/// string [CustomerId, CustomerName]
/// CustomerDiscountClass [Discount]
/// </code>
/// </summary>
public class DTO5_1
{
    public string? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public CustomerDiscountClass? Discount { get; set; }
}

/// <summary>
/// Represents a customer for POST.
/// <code>
/// DateTime [Birthday]
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
    public CustomerDiscountClass? Discount { get; set; }
}

/// <summary>
/// Represents a customer for GET by id and PUT for customer and discount.
/// <code>
/// DateTime [Birthday]
/// string [CustomerId, CustomerFullName, CustomerAddress, CustomerPhone, CustomerMail, CustomerNIT, CustomerNIP, CustomerOccupation, CustomerOrganizationName]
/// CustomerDiscountClass [Discount]
/// </code>
/// </summary>
public class DTO5_3
{
    public DateTime? Birthday { get; set; }
    public string? CustomerId { get; set; }
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
/// Represents a customer for PUT by discount.
/// <code>
/// string [CustomerId]
/// int [DiscountId]
/// </code>
/// </summary>
public class DTO5_4
{
    public string? CustomerId { get; set; }
    public int DiscountId { get; set; }
}
#endregion

#region Sellers
/// <summary>
/// Represents a seller for GET.
/// string [Id, FullName]
/// </summary>
public class DTO6
{
    public string? Id { get; set; }
    public string? FullName { get; set; }
}

/// <summary>
/// Represents a seller for POST.
/// DateTime [Birthday]
/// string [FullName, Address, Phone, Mail, NIT, NIP, Occupation]
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
/// Represents a seller for Get by Id and PUT.
/// <code>
/// DateTime [Birthday]
/// string [Id, FullName, Address, Phone, Mail, NIT, NIP, Occupation]
/// </code>
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
