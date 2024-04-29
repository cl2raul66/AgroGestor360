using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Expense;
using AgroGestor360.App.Views.Loans;
using AgroGestor360.App.Views.Settings;
using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.App.Views.Settings.Connection;
using AgroGestor360.App.Views.Settings.Customers;
using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.App.Views.Settings.Shareholders;
using AgroGestor360.App.Views.Settings.Warehouse;

namespace AgroGestor360.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PgHome), typeof(PgHome));
        Routing.RegisterRoute(nameof(PgSettings), typeof(PgSettings));
        Routing.RegisterRoute(nameof(PgSetURL), typeof(PgSetURL));
        Routing.RegisterRoute(nameof(PgAddAccountOrCard), typeof(PgAddAccountOrCard));
        Routing.RegisterRoute(nameof(PgAddEditShareholder), typeof(PgAddEditShareholder));
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
    }
}
