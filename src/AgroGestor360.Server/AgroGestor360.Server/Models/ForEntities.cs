using AgroGestor360.Server.Tools.Enums;
using LiteDB;
using vCardLib.Models;

namespace AgroGestor360.Server.Models;

/// <summary>
/// Objeto: Organization => Representa una organización en el sistema.
/// <code>
/// string [Id] => Identificador único de la organización
/// string [Name] => Nombre de la organización
/// string [Address] => Dirección de la organización
/// string [Phone] => Número de teléfono de la organización
/// string [Email] => Correo electrónico de la organización
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
/// Objeto: ClientDevice => Representa un dispositivo del cliente.
/// <code>
/// ObjectId [Id] => Identificador único del dispositivo
/// DeviceInfo [Device] => Información detallada del dispositivo
/// </code>
/// </summary>
public class ClientDevice
{
    public ObjectId? Id { get; set; }
    public DeviceInfo? Device { get; set; }
}

/// <summary>
/// Objeto: DeviceInfo => Representa la información detallada de un dispositivo.
/// <code>
/// string [Brand] => Marca del dispositivo
/// string [Model] => Modelo del dispositivo
/// string [SerialNumber] => Número de serie del dispositivo
/// string [SO] => Sistema operativo del dispositivo
/// </code>
/// </summary>
public class DeviceInfo
{
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? SO { get; set; }
}

/// <summary>
/// Objeto: User => Representa un usuario en el sistema.
/// <code>
/// ObjectId [Id] => Identificador único del usuario
/// vCard [Contact] => Información de contacto del usuario
/// bool [IsAuthorized] => Indica si el usuario está autorizado
/// List of string [GroupsId] => Lista de identificadores de grupos a los que pertenece el usuario
/// </code>
/// </summary>
public class User
{
    public ObjectId? Id { get; set; }
    public vCard? Contact { get; set; }
    public bool IsAuthorized { get; set; }
    public List<string>? GroupsId { get; set; }
}

/// <summary>
/// Objeto: UserGroup => Representa un grupo de usuarios en el sistema.
/// <code>
/// string [Id] => Identificador único del grupo
/// string [Name] => Nombre del grupo
/// </code>
/// </summary>
public class UserGroup
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

/// <summary>
/// Objeto: Seller => Representa un vendedor en el sistema.
/// <code>
/// ObjectId [Id] => Identificador único del vendedor
/// vCard [Contact] => Información de contacto del vendedor
/// </code>
/// </summary>
public class Seller
{
    public ObjectId? Id { get; set; }
    public vCard? Contact { get; set; }
}

/// <summary>
/// Objeto: Customer => Representa un cliente en el sistema.
/// <code>
/// ObjectId [Id] => Identificador único del cliente
/// vCard [Contact] => Información de contacto del cliente
/// DiscountForCustomer [Discount] => Descuento aplicable al cliente
/// LineCredit [Credit] => Línea de crédito del cliente
/// </code>
/// </summary>
public class Customer
{
    public ObjectId? Id { get; set; }
    public vCard? Contact { get; set; }
    public DiscountForCustomer? Discount { get; set; }
    public CustomerLineCredit? Credit { get; set; }
}

/// <summary>
/// Objeto: DiscountForCustomer => Representa un descuento aplicable a un cliente.
/// <code>
/// int [Id] => Identificador único del descuento
/// string [Name] => Nombre del descuento
/// double [Discount] => Porcentaje o monto del descuento
/// </code>
/// </summary>
public class DiscountForCustomer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Discount { get; set; }
}

/// <summary>
/// Objeto: LineCredit => Representa una línea de crédito básica.
/// <code>
/// int [Id] => Identificador único de la línea de crédito
/// string [Name] => Nombre de la línea de crédito
/// double [Amount] => Monto total de la línea de crédito
/// </code>
/// </summary>
public class LineCredit
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Amount { get; set; }
}

/// <summary>
/// Objeto: CustomerLineCredit => Representa una línea de crédito específica para un cliente.
/// <code>
/// int [Id] => Identificador único de la línea de crédito
/// string [Name] => Nombre de la línea de crédito
/// double [Amount] => Monto total de la línea de crédito
/// int [TimeLimit] => Límite de tiempo en días
/// </code>
/// </summary>
public class CustomerLineCredit : LineCredit
{
    public int TimeLimit { get; set; }
}

/// <summary>
/// Objeto: TimeLimitForCredit => Representa un límite de tiempo para créditos.
/// <code>
/// int [Id] => Identificador único del límite de tiempo
/// int [TimeLimit] => Límite de tiempo en días
/// </code>
/// </summary>
public class TimeLimitForCredit
{
    public int Id { get; set; }
    public int TimeLimit { get; set; }
}

/// <summary>
/// Objeto: MerchandiseItem => Representa un artículo de mercancía.
/// <code>
/// ObjectId [Id] => Identificador único del artículo
/// string [Name] => Nombre del artículo
/// string [Category] => Categoría del artículo
/// string [Description] => Descripción del artículo
/// Presentation [Packaging] => Presentación o empaque del artículo
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

/// <summary>
/// Objeto: Presentation => Representa la presentación o empaque de un artículo.
/// <code>
/// string [Measure] => Tipo de medida (ej. peso, volumen)
/// string [Unit] => Unidad de medida
/// double [Value] => Valor de la medida
/// </code>
/// </summary>
public class Presentation
{
    public string? Measure { get; set; }
    public string? Unit { get; set; }
    public double Value { get; set; }
}

/// <summary>
/// Objeto: ArticleItemForWarehouse => Representa un artículo en el almacén.
/// <code>
/// ObjectId [MerchandiseId] => Identificador único de la mercancía
/// string [MerchandiseName] => Nombre de la mercancía
/// double [Quantity] => Cantidad disponible
/// double [Reserved] => Cantidad reservada
/// Presentation [Packaging] => Presentación o empaque del artículo
/// </code>
/// </summary>
public class ArticleItemForWarehouse
{
    public double Quantity { get; set; }
    public double Reserved { get; set; }
    public ObjectId? MerchandiseId { get; set; }
    public string? MerchandiseName { get; set; }
    public Presentation? Packaging { get; set; }
}

/// <summary>
/// Objeto: ArticleItemForSale => Representa un artículo para venta.
/// <code>
/// ObjectId [MerchandiseId] => Identificador único de la mercancía
/// string [MerchandiseName] => Nombre de la mercancía
/// Presentation [Packaging] => Presentación o empaque del artículo
/// double [Price] => Precio de venta
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
/// Objeto: ProductItemForSale => Representa un producto para venta.
/// <code>
/// ObjectId [Id] => Identificador único del producto
/// ObjectId [MerchandiseId] => Identificador único de la mercancía asociada
/// string [ProductName] => Nombre del producto
/// double [ProductQuantity] => Cantidad del producto
/// double [ArticlePrice] => Precio del artículo
/// Presentation [Packaging] => Presentación o empaque del producto
/// ProductOffering[] [Offering] => Ofertas asociadas al producto
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

/// <summary>
/// Objeto: ProductOffering => Representa una oferta para un producto.
/// <code>
/// int [Id] => Identificador único de la oferta
/// double [Quantity] => Cantidad de la oferta
/// double [BonusAmount] => Monto de bonificación
/// </code>
/// </summary>
public class ProductOffering
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    public double BonusAmount { get; set; }
}

/// <summary>
/// Objeto: BankAccount => Representa una cuenta bancaria para un registro contable.
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
/// Objeto: PaymentMethod => Representa un pago en el sistema.
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
/// Objeto: Loan => Representa un préstamo en el sistema.
/// <code>
/// ObjectId [Id] => Identificador único del préstamo
/// DateTime [Date] => Fecha del préstamo
/// string [LoanNumber] => Número de préstamo
/// ObjectId [BankAccountId] => Identificador de la cuenta bancaria asociada
/// double [Amount] => Monto del préstamo
/// double [Interest] => Tasa de interés
/// string [Concept] => Concepto o propósito del préstamo
/// LoanType [Type] => Tipo de préstamo
/// double [Insurance] => Monto del seguro
/// string [MoreDetails] => Detalles adicionales
/// </code>
/// </summary>
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

/// <summary>
/// Objeto: Expense => Representa un gasto en el sistema.
/// <code>
/// ObjectId [Id] => Identificador único del gasto
/// </code>
/// </summary>
public class Expense
{
    public ObjectId? Id { get; set; }
}

/// <summary>
/// Objeto: BankTransaction => Representa una transacción bancaria para un registro contable.
/// <code>
/// ObjectId [BankAccountId] => Identificador de la cuenta bancaria
/// double [TransactionAmount] => Monto de la transacción
/// </code>
/// </summary>
public class BankTransaction
{
    public ObjectId? BankAccountId { get; set; }
    public double TransactionAmount { get; set; }
}

/// <summary>
/// Objeto: SaleBase => Representa la base común para todas las etapas de venta.
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
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
/// Objeto: Quotation => Representa una cotización (etapa de preventa).
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// Status [QuotationStatus] => Estado de la cotización
/// </code>
/// </summary>
public class Quotation : SaleBase
{
    public QuotationStatus Status { get; set; }
}

/// <summary>
/// Object: Order => Representa un pedido (etapa de preventa).
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// OrderStatus [Status] => Estado del pedido
/// </code>
/// </summary>
public class Order : SaleBase
{
    public OrderStatus Status { get; set; }
}

/// <summary>
/// Object: SaleRecord => Representa el registro de una venta realizada.
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// SaleStatus [Status] => Estado de la venta
/// string [NumberFEL] => Número de factura (si aplica)
/// Array PaymentMethod [Payments] => Métodos de pago utilizados
/// </code>
/// </summary>
public class SaleRecord : SaleBase
{
    public SaleStatus Status { get; set; }
    public string? NumberFEL { get; set; }
    public PaymentMethod[]? PaymentMethods { get; set; }
}

/// <summary>
/// Object: WasteSaleRecord => Representa el registro de una venta de desecho.
/// <code>
/// DateTime [Date] => Fecha del documento
/// string [Code] => Código único del documento
/// Seller [Seller] => Vendedor asociado
/// Customer [Customer] => Cliente asociado
/// Array ProductSaleBase [Products] => Productos incluidos en la venta
/// SaleStatus [Status] => Estado de la venta
/// string [NumberFEL] => Número de factura (si aplica)
/// Array PaymentMethod [Payments] => Métodos de pago utilizados
/// string [Notes] => Notas adicionales
/// </code>
/// </summary>
public class WasteSaleRecord : SaleRecord
{
    public string? Notes { get; set; }
}

/// <summary>
/// Objeto: ConceptForDeletedSaleRecord => Representa un concepto para una factura eliminada.
/// <code>
/// int [Id] => Identificador único del concepto
/// string [Concept] => Descripción del concepto
/// </code>
/// </summary>
public class ConceptForDeletedSaleRecord
{
    public int Id { get; set; }
    public string? Concept { get; set; }
}
