namespace AgroGestor360.Server.Tools.Enums;

public enum OperationType { Create, Delete, Update }

public enum GroupName { GroupSender, GroupReceiver }

public enum ServerStatus { Running, Stopped }

public enum TransactionType { Sale, Expense, Loan, ShareholderContribution }

public enum LoanType { Fiduciary, Mortgage, Pledge, CreditCard, Lender }

public enum ImmediatePaymentType { Cash, Card, BankTransfer, MobilePayment, CustomerAccount }

public enum CreditPaymentType { Check, CreditCard, CustomerAccount }

public enum ExpenseType { Tax, NotTax, Payroll }

public enum FinancialInstrumentType { Current, Savings, Investment, Loan, Payroll, CreditCard, DebitCard }
