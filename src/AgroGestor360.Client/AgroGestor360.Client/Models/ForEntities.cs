// Ignore Spelling: DTO

using AgroGestor360.Client.Tools;
using vCardLib.Models;

namespace AgroGestor360.Client.Models;

public class Organization
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class MerchandiseCategory
{
    public string? Name { get; set; }
}

public class Presentation
{
    public string? Measure { get; set; }
    public string? Unit { get; set; }
    public double Value { get; set; }
}

public class ProductOffering
{
    public int? Id { get; set; }
    public double Quantity { get; set; }
    public double BonusAmount { get; set; }
}

public class CustomerDiscountClass
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Value { get; set; }
}

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

public class BankAccount
{
    public string? Number { get; set; }
    public string? BankName { get; set; }
    public string? Alias { get; set; }
    public vCard? Beneficiary { get; set; }
    public FinancialInstrumentType InstrumentType { get; set; }
}

public class Bank
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}