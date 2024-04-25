// Ignore Spelling: DTO

using AgroGestor360.Server.Tools.Enums;

namespace AgroGestor360.Server.Models;

#region Merchandise
/// <summary>
/// Object: MerchandiseItem
/// <para>
/// string [Id, Name, Description], Presentation [Packaging], MerchandiseCategory [Category]
/// </para>
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
/// Object: ArticleItemForWarehouse
/// <para>
/// string [Id], DTO1 [Merchandise], double [Quantity]
/// </para>
/// </summary>
public class DTO2
{
    public string? Id { get; set; }
    public DTO1? Merchandise { get; set; }
    public double Quantity { get; set; }
}

/// <summary>
/// Object: ArticleItemForWarehouse 
/// <para>
/// string [Id, Name, Unit,  Category], double [Value, Quantity]
/// </para>
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
/// Object: ArticleItemForSale 
/// <para>
/// string [Id], DTO1 [Merchandise], double [Price] 
/// </para>
/// </summary>
public class DTO3
{
    public string? Id { get; set; }
    public DTO1? Merchandise { get; set; }
    public double Price { get; set; }
}

/// <summary>
/// Object: ArticleItemForSale 
/// <para>
/// string [Id, Name, Unit, Category], double [Value, Price] 
/// </para>
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
/// Object: ProductItemForSale 
/// <para>
/// string [Id], DTO3 [Article], double [Quantity], List ProductOffering [Offering]
/// </para>
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
/// Object: ProductItemForSale 
/// <para>
/// string [Id, Name], double [Quantity, SalePrice]
/// </para>
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
/// Object: ProductItemForSale 
/// <para>
/// string [Id], double [Quantity]
/// </para>
/// </summary>
public class DTO4_2
{
    public string? Id { get; set; }
    public double Quantity { get; set; }
}

/// <summary>
/// Object: ProductItemForSale 
/// <para>
/// string [Id], List ProductOffering [Offering]
/// </para>
/// </summary>
public class DTO4_3
{
    public string? Id { get; set; }
    public List<ProductOffering>? Offering { get; set; }
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
/// <para>
/// string [CustomerFullName, CustomerAddress, CustomerPhone, CustomerMail, CustomerNIT, CustomerNIP, CustomerOccupation, CustomerOrganizationName], CustomerDiscountClass [Discount]
/// </para>
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
