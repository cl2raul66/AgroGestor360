using AgroGestor360App.Views;
using AgroGestor360App.Views.Dialogs;
using AgroGestor360App.Views.Expense;
using AgroGestor360App.Views.Loans;
using AgroGestor360App.Views.Sales;
using AgroGestor360App.Views.Settings;

namespace AgroGestor360App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PgAuthenticationDialog), typeof(PgAuthenticationDialog));
        Routing.RegisterRoute(nameof(PgSelectDiscountsOptionsDialog), typeof(PgSelectDiscountsOptionsDialog));
        Routing.RegisterRoute(nameof(PgHome), typeof(PgHome));
        Routing.RegisterRoute(nameof(PgSettings), typeof(PgSettings));
        Routing.RegisterRoute(nameof(PgSetURL), typeof(PgSetURL));
        Routing.RegisterRoute(nameof(PgAddAccountOrCard), typeof(PgAddAccountOrCard));
        Routing.RegisterRoute(nameof(PgAddDiscountForCustomer), typeof(PgAddDiscountForCustomer));
        Routing.RegisterRoute(nameof(PgAddLineCredit), typeof(PgAddLineCredit));
        Routing.RegisterRoute(nameof(PgAdminTimeLimit), typeof(PgAdminTimeLimit));
        Routing.RegisterRoute(nameof(PgSetDefaultTimeLimit), typeof(PgSetDefaultTimeLimit));
        Routing.RegisterRoute(nameof(PgSetCreditForCustomer), typeof(PgSetCreditForCustomer));
        Routing.RegisterRoute(nameof(PgAddEditWarehouse), typeof(PgAddEditWarehouse));
        Routing.RegisterRoute(nameof(PgAddProduct), typeof(PgAddProduct));
        Routing.RegisterRoute(nameof(PgCreateOffer), typeof(PgCreateOffer));
        Routing.RegisterRoute(nameof(PgAddEditCustomer), typeof(PgAddEditCustomer));
        Routing.RegisterRoute(nameof(PgAddEditSeller), typeof(PgAddEditSeller));
        Routing.RegisterRoute(nameof(PgLoans), typeof(PgLoans));
        Routing.RegisterRoute(nameof(PgAddLoan), typeof(PgAddLoan));
        Routing.RegisterRoute(nameof(PgAmortization), typeof(PgAmortization));
        Routing.RegisterRoute(nameof(PgExpense), typeof(PgExpense));
        Routing.RegisterRoute(nameof(PgAddExpense), typeof(PgAddExpense));
        Routing.RegisterRoute(nameof(PgSales), typeof(PgSales));
        Routing.RegisterRoute(nameof(PgAddEditQuote), typeof(PgAddEditQuote));
        Routing.RegisterRoute(nameof(PgAddEditOrder), typeof(PgAddEditOrder));
        Routing.RegisterRoute(nameof(PgAddEditSale), typeof(PgAddEditSale));
        Routing.RegisterRoute(nameof(PgDeletedInSale), typeof(PgDeletedInSale));
        Routing.RegisterRoute(nameof(PgDeletedInvoice), typeof(PgDeletedInvoice));
        Routing.RegisterRoute(nameof(PgTotalOrPartialPayment), typeof(PgTotalOrPartialPayment));
        Routing.RegisterRoute(nameof(PgReports), typeof(PgReports));
    }
}
