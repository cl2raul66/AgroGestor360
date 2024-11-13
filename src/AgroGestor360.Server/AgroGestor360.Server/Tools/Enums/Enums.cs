namespace AgroGestor360.Server.Tools.Enums;

public enum OperationType { Create, Delete, Update }

public enum GroupName { GroupSender, GroupReceiver }

public enum ServerStatus { Running, Stopped }

public enum TypeFrequencyReconciliationPolicy { Daily, Weekly, Monthly, ShiftChange }

public enum TypeDiscrepancy { None, NoUserAmount, Missing, Leftover }

/// <summary>
/// Representa el tipo de transacción en el sistema.
/// <code>
/// Sale: Transacción de venta.
/// Expense: Transacción de gasto.
/// Loan: Transacción de préstamo.
/// ShareholderContribution: Transacción de aporte de accionista.
/// </code>
/// </summary>
public enum TransactionType { Sale, Expense, Loan, ShareholderContribution }

/// <summary>
/// Representa el tipo de préstamo en el sistema.
/// <code>
/// Fiduciary: Préstamo fiduciaria.
/// Mortgage: Préstamo hipotecaria.
/// Pledge: Préstamo prendaria.
/// CreditCard: Préstamo de tarjeta de crédito.
/// Lender: Préstamo de prestamista.
/// </code>
/// </summary>
public enum LoanType { Fiduciary, Mortgage, Pledge, CreditCard, Lender }

/// <summary>
/// Representa el tipo de pago en el sistema.
/// <code>
/// Cash: Pago realizado en efectivo.
/// DebitCard: Pago realizado con tarjeta de débito.
/// CreditCard: Pago realizado con tarjeta de crédito.
/// BankTransfer: Pago realizado mediante transferencia bancaria.
/// BankDeposit: Pago realizado mediante deposito bancario.
/// Check: Pago realizado mediante cheque.
/// </code>
/// </summary>
public enum PaymentType
{
    Cash,
    DebitCard,
    CreditCard,
    BankTransfer,
    BankDeposit,
    Check
}

/// <summary>
/// PaymentCondition: Representa la condición de pago en el sistema.
/// <code>
/// Immediate: El pago se realiza de forma inmediata.
/// Credit: El pago se realiza a crédito o de forma diferida.
/// </code>
/// </summary>
public enum PaymentCondition
{
    Immediate,
    Credit
}

/// <summary>
/// Representa el tipo de gasto en el sistema.
/// <code>
/// Tax: Gasto que se debe pagar al gobierno.
/// NotTax: Gasto que no se debe pagar al gobierno.
/// Payroll: Gasto que se debe pagar a los empleados.
/// </code>
/// </summary>
public enum ExpenseType { Tax, NotTax, Payroll }

/// <summary>
/// Representa el tipo de instrumento financiero en el sistema.
/// <code>
/// Current: Cuenta corriente.
/// Savings: Cuenta de ahorros.
/// Investment: Cuenta de inversión.
/// Payroll: Cuenta de nómina.
/// CreditCard: Tarjeta de crédito.
/// DebitCard: Tarjeta de débito.
/// </code>
/// </summary>
public enum FinancialInstrumentType { Current, Savings, Investment, Payroll, CreditCard, DebitCard }

/// <summary>
/// Representa el estado de una cotización en el sistema.
/// <code>
/// Draft: La cotización ha sido creada pero no ha sido enviada al cliente.
/// Sent: La cotización ha sido enviada al cliente.
/// Accepted: La cotización ha sido aceptada por el cliente.
/// Rejected: La cotización ha sido rechazada por el cliente.
/// Cancelled: La cotización ha sido cancelada por el sistema.
/// </code>
/// </summary>
public enum QuotationStatus { Draft, Sent, Accepted, Rejected, Cancelled }

/// <summary>
/// Representa el estado de un pedido en el sistema.
/// <code>
/// Pending: El pedido ha sido creado pero no ha sido procesado.
/// Processing: El pedido ha sido procesado y está en proceso de ser completado.
/// Completed: El pedido ha sido completado y entregado al cliente.
/// Rejected: La cotización ha sido rechazada por el cliente.
/// Cancelled: El pedido ha sido cancelado por el sistema.
/// </code>
/// </summary>
public enum OrderStatus { Pending, Processing, Completed, Rejected, Cancelled }

/// <summary>
/// Representa el estado de una venta en el sistema.
/// <code>
/// Paid: La venta ha sido completamente pagada.
/// Pending: La venta está pendiente de pago o facturación.
/// Cancelled: La venta ha sido cancelada por el cliente o por el sistema.
/// </code>
/// </summary>
public enum SaleStatus { Paid, Pending, Cancelled }
