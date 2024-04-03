using AgroGestor360.Server.Tools.Enums;
using vCardLib.Models;

namespace AgroGestor360.Server.Models;

//public class ClientDeviceDTO
//{
//    public string? Id { get; set; }
//    public DeviceInfo? Device { get; set; }
//}

//public class DeviceInfoDTO
//{
//    public string? Brand { get; set; }
//    public string? Model { get; set; }
//    public string? SerialNumber { get; set; }
//    public string? SO { get; set; }
//}

//public class UserDTO
//{
//    public string? Id { get; set; }
//    public vCard? Contact { get; set; }
//    public bool IsAuthorized { get; set; }
//    public List<string>? GroupsId { get; set; }
//}

//public class ClientClassDTO
//{
//    public string? Id { get; set; }
//    public string? Title { get; set; }
//    public double? PercentageValue { get; set; }
//}

//public class UserGroupDTO
//{
//    public string? Id { get; set; }
//    public string? Name { get; set; }
//}

//public class CustomerDiscountDTO
//{
//    public DateTime Date { get; set; }
//    public string? CustomerId { get; set; }
//    public string? CustomerDiscountClass { get; set; }
//}

//public class CustomerDiscountClassDTO
//{
//    public string? Id { get; set; }
//    public string? Name { get; set; }
//    public double Value { get; set; }
//}

//public class MerchandiseDTO
//{
//    public string? Id { get; set; }
//    public string? Name { get; set; }
//    public Presentation? Packaging { get; set; }
//    public string? Category { get; set; }
//    public string? Description { get; set; }
//}

//public class PresentationDTO
//{
//    public string? Measure { get; set; }
//    public string? Unit { get; set; }
//    public double Value { get; set; }
//}

//public class ArticleDTO
//{
//    public string? Id { get; set; }
//    public string? MerchandiseId { get; set; }
//    public double Price { get; set; }
//}

//public class ProductDTO
//{
//    public string? Id { get; set; }
//    public string? ArticlesId { get; set; }
//    public double Quantity { get; set; }
//}

//public class ProductOfferingDTO
//{
//    public string? Id { get; set; }
//    public string? ProductId { get; set; }
//    public double Quantity { get; set; }
//    public double BonusAmount { get; set; }
//}

public class BankAccountDTO
{
    public string? Number { get; set; }
    public string? BankName { get; set; }
    public string? Alias { get; set; }
    public vCard? Beneficiary { get; set; }
    public FinancialInstrumentType InstrumentType { get; set; }
}

public class BankDTO
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

//public class SaleDTO
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

//public class ImmediatePaymentDTO : PaymentMethod
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

//public class SaleProductDTO
//{
//    public string? ProductId { get; set; }
//    public double Quantity { get; set; }
//    public string? ProductOfferingId { get; set; }
//    public string? CustomerDiscountClassId { get; set; }
//}

//public class LoanDTO
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

//public class ExpenseDTO
//{
//    public string? Id { get; set; }
//}

//public class BankTransactionDTO
//{
//    public string? BankAccountId { get; set; }
//    public double TransactionAmount { get; set; }
//}

//public class TokenDTO
//{
//    public string? Id { get; set; }
//}
