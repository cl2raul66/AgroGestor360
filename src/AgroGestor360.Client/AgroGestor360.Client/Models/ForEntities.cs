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

//public class ClientDevice
//{
//    public string? Id { get; set; }
//    public DeviceInfo? Device { get; set; }
//}

//public class DeviceInfo
//{
//    public string? Brand { get; set; }
//    public string? Model { get; set; }
//    public string? SerialNumber { get; set; }
//    public string? SO { get; set; }
//}

//public class User
//{
//    public string? Id { get; set; }
//    public vCard? Contact { get; set; }
//    public bool IsAuthorized { get; set; }
//    public List<string>? GroupsId { get; set; }
//}

//public class ClientClass
//{
//    public string? Id { get; set; }
//    public string? Title { get; set; }
//    public double? PercentageValue { get; set; }
//}

//public class UserGroup
//{
//    public string? Id { get; set; }
//    public string? Name { get; set; }
//}

//public class CustomerDiscount
//{
//    public DateTime Date { get; set; }
//    public string? CustomerId { get; set; }
//    public string? CustomerDiscountClass { get; set; }
//}

//public class CustomerDiscountClass
//{
//    public string? Id { get; set; }
//    public string? Name { get; set; }
//    public double Value { get; set; }
//}

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
    public double Quantity { get; set; }
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

//public class Sale
//{
//    public string? Id { get; set; }
//    public List<SaleProduct>? SaleProducts { get; set; }
//    public string? SellerId { get; set; }
//    public string? CustomerId { get; set; }
//    public bool IsWithFEL { get; set; }
//    public List<PaymentMethod>? Payments { get; set; }
//}

//public abstract class PaymentMethod
//{
//    public double Amount { get; set; }
//}

//public class ImmediatePayment : PaymentMethod
//{
//    public string? Reference { get; set; }
//    public string? BankAccountId { get; set; }
//    public ImmediatePaymentType Type { get; set; }
//}

//public class CreditPayment : PaymentMethod
//{
//    public CreditPaymentType Type { get; set; }
//    public int NumberOfInstallments { get; set; }
//}

//public class SaleProduct
//{
//    public string? ProductId { get; set; }
//    public double Quantity { get; set; }
//    public string? ProductOfferingId { get; set; }
//    public string? CustomerDiscountClassId { get; set; }
//}

//public class Loan
//{
//    public string? Id { get; set; }
//    public DateTime Date { get; set; }
//    public string? LoanNumber { get; set; }
//    public string? BankAccountId { get; set; }
//    public double Amount { get; set; }
//    public double Interest { get; set; }
//    public string? Concept { get; set; }
//    public LoanType Type { get; set; }
//    public double Insurance { get; set; }
//    public string? MoreDetails { get; set; }
//}

//public class Expense
//{
//    public string? Id { get; set; }
//}

//public class BankTransaction
//{
//    public string? BankAccountId { get; set; }
//    public double TransactionAmount { get; set; }
//}

//public class Token
//{
//    public string? Id { get; set; }
//}
