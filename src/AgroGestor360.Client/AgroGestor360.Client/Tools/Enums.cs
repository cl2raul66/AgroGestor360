namespace AgroGestor360.Client.Tools;

public enum OperationType { Create, Delete, Update }

public enum GroupName { GroupSender, GroupReceiver }

public enum ServerStatus { Running, Stopped }

public enum TransactionType { Sale, Expense, Loan, ShareholderContribution }

public enum LoanType { Fiduciary, Mortgage, Pledge, CreditCard, Lender }

public enum ImmediatePaymentType { Cash, Card, BankTransfer, MobilePayment, CustomerAccount }

public enum CreditPaymentType { Check, CreditCard, CustomerAccount }

public enum ExpenseType { Tax, NotTax, Payroll }

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
/// </summary>AcceptedAccepted
public enum QuotationStatus { Draft, Sent, Accepted, Rejected, Cancelled }

/// <summary>
/// Representa el estado de un pedido en el sistema.
/// <code>
/// Pending: El pedido ha sido creado pero no ha sido procesado.
/// Processing: El pedido ha sido procesado y está en proceso de ser completado.
/// Completed: El pedido ha sido completado y entregado al cliente.
/// Cancelled: El pedido ha sido cancelado por el cliente o por el sistema.
/// </code>
/// </summary>
public enum OrderStatus { Pending, Processing, Completed, Cancelled }

/// <summary>
/// Representa el estado de una factura en el sistema.
/// <code>
/// Paid: La factura ha sido pagada por el cliente.
/// Pending: La factura ha sido creada pero no ha sido pagada.
/// Cancelled: La factura ha sido cancelada por el cliente o por el sistema.
/// </code>
/// </summary>
public enum InvoiceStatus { Paid, Pending, Cancelled }
