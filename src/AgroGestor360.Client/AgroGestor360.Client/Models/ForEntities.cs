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
//    public ObjectId? Id { get; set; }
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
//    public ObjectId? Id { get; set; }
//    public vCard? Contact { get; set; }
//    public bool IsAuthorized { get; set; }
//    public List<string>? GroupsId { get; set; }
//}

//public class ClientClass
//{
//    public ObjectId? Id { get; set; }
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
//    public ObjectId? CustomerId { get; set; }
//    public ObjectId? CustomerDiscountClass { get; set; }
//}

//public class CustomerDiscountClass
//{
//    public ObjectId? Id { get; set; }
//    public string? Name { get; set; }
//    public double Value { get; set; }
//}

//public class Merchandise
//{
//    public ObjectId? Id { get; set; }
//    public string? Name { get; set; }
//    public Presentation? Packaging { get; set; }
//    public string? Category { get; set; }
//    public string? Description { get; set; }
//}

//public class Presentation
//{
//    public string? Measure { get; set; }
//    public string? Unit { get; set; }
//    public double Value { get; set; }
//}

//public class Article
//{
//    public ObjectId? Id { get; set; }
//    public ObjectId? MerchandiseId { get; set; }
//    public double Price { get; set; }
//}

//public class Product
//{
//    public ObjectId? Id { get; set; }
//    public ObjectId? ArticlesId { get; set; }
//    public double Quantity { get; set; }
//}

//public class ProductOffering
//{
//    public ObjectId? Id { get; set; }
//    public ObjectId? ProductId { get; set; }
//    public double Quantity { get; set; }
//    public double BonusAmount { get; set; }
//}

//public class BankAccount
//{
//    public ObjectId? Id { get; set; }
//    public ObjectId? BankId { get; set; }
//    public string? Alias { get; set; }
//    public vCard? Beneficiary { get; set; }
//    public FinancialInstrumentType InstrumentType { get; set; }
//    public bool Enable {get; set;}
//}

//public class Bank
//{
//    public ObjectId? Id { get; set; }
//    public string? Name { get; set; }
//}

//public class Sale
//{
//    public ObjectId? Id { get; set; }
//    public List<SaleProduct>? SaleProducts { get; set; }
//    public ObjectId? SellerId { get; set; }
//    public ObjectId? CustomerId { get; set; }
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
//    public ObjectId? BankAccountId { get; set; }
//    public ImmediatePaymentType Type { get; set; }
//}

//public class CreditPayment : PaymentMethod
//{
//    public CreditPaymentType Type { get; set; }
//    public int NumberOfInstallments { get; set; }
//}

//public class SaleProduct
//{
//    public ObjectId? ProductId { get; set; }
//    public double Quantity { get; set; }
//    public ObjectId? ProductOfferingId { get; set; }
//    public ObjectId? CustomerDiscountClassId { get; set; }
//}

//public class Loan
//{
//    public ObjectId? Id { get; set; }
//    public DateTime Date { get; set; }
//    public string? LoanNumber { get; set; }
//    public ObjectId? BankAccountId { get; set; }
//    public double Amount { get; set; }
//    public double Interest { get; set; }
//    public string? Concept { get; set; }
//    public LoanType Type { get; set; }
//    public double Insurance { get; set; }
//    public string? MoreDetails { get; set; }
//}

//public class Expense
//{
//    public ObjectId? Id { get; set; }
//}

//public class BankTransaction
//{
//    public ObjectId? BankAccountId { get; set; }
//    public double TransactionAmount { get; set; }
//}

//public class Token
//{
//    public string? Id { get; set; }
//}
