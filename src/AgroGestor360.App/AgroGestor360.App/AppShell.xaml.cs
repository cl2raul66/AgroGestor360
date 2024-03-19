using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Expense;
using AgroGestor360.App.Views.Loans;
using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.App.Views.Settings.Customers;
using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.App.Views.Settings.Shareholders;

namespace AgroGestor360.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PgHome), typeof(PgHome));
        Routing.RegisterRoute(nameof(PgSettings), typeof(PgSettings));
        Routing.RegisterRoute(nameof(PgAddAccountOrCard), typeof(PgAddAccountOrCard));
        Routing.RegisterRoute(nameof(PgAddEditShareholder), typeof(PgAddEditShareholder));
        Routing.RegisterRoute(nameof(PgAddItem), typeof(PgAddItem));
        Routing.RegisterRoute(nameof(PgAddProduct), typeof(PgAddProduct));
        Routing.RegisterRoute(nameof(PgCreateOffer), typeof(PgCreateOffer));
        Routing.RegisterRoute(nameof(PgAddEditCustomer), typeof(PgAddEditCustomer));
        Routing.RegisterRoute(nameof(PgLoans), typeof(PgLoans));
        Routing.RegisterRoute(nameof(PgAddLoan), typeof(PgAddLoan));
        Routing.RegisterRoute(nameof(PgAmortization), typeof(PgAmortization));
        Routing.RegisterRoute(nameof(PgExpense), typeof(PgExpense));
        Routing.RegisterRoute(nameof(PgAddExpense), typeof(PgAddExpense));
        Routing.RegisterRoute(nameof(PgSales), typeof(PgSales));
    }
}
