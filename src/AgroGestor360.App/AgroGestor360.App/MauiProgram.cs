using AgroGestor360.App.Services;
using AgroGestor360.App.View.Settings;
using AgroGestor360.App.ViewModels;
using AgroGestor360.App.ViewModels.Expense;
using AgroGestor360.App.ViewModels.Loans;
using AgroGestor360.App.ViewModels.Settings;
using AgroGestor360.App.ViewModels.Settings.BankAccounts;
using AgroGestor360.App.ViewModels.Settings.Customers;
using AgroGestor360.App.ViewModels.Settings.Products;
using AgroGestor360.App.ViewModels.Settings.Sales;
using AgroGestor360.App.ViewModels.Settings.Shareholders;
using AgroGestor360.App.ViewModels.Settings.Warehouse;
using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Expense;
using AgroGestor360.App.Views.Loans;
using AgroGestor360.App.Views.Settings;
using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.App.Views.Settings.Customers;
using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.App.Views.Settings.Sales;
using AgroGestor360.App.Views.Settings.Shareholders;
using AgroGestor360.App.Views.Settings.Warehouse;
using AgroGestor360.Client.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AgroGestor360.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("icofont.ttf", "icofont");
            });

        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<PgSignIn, PgSignInViewModel>();
        builder.Services.AddTransient<PgHome, PgHomeViewModel>();
        builder.Services.AddTransient<PgSettings, PgSettingsViewModel>();
        builder.Services.AddTransient<CvConnection, CvConnectionViewModel>();
        builder.Services.AddTransient<CvSeedCapital, CvSeedCapitalViewModel>();
        builder.Services.AddTransient<CvBankAccounts, CvBankAccountsViewModel>();
        builder.Services.AddTransient<PgAddAccountOrCard, PgAddAccountOrCardViewModel>();
        builder.Services.AddTransient<CvShareholders, CvShareholdersViewModel>();
        builder.Services.AddTransient<PgAddEditShareholder, PgAddEditShareholderViewModel>();
        builder.Services.AddTransient<CvProducts, CvProductsViewModel>();
        builder.Services.AddTransient<PgAddProduct, PgAddProductViewModel>();
        builder.Services.AddTransient<PgCreateOffer, PgCreateOfferViewModel>();
        builder.Services.AddTransient<CvCustomers, CvCustomersViewModel>();
        builder.Services.AddTransient<PgAddEditCustomer, PgAddEditCustomerViewModel>();
        builder.Services.AddTransient<CvWarehouse, CvWarehouseViewModel>();
        builder.Services.AddTransient<PgAddMerchandise, PgAddMerchandiseViewModel>();
        builder.Services.AddTransient<PgLoans, PgLoansViewModel>();
        builder.Services.AddTransient<PgAddLoan, PgAddLoanViewModel>();
        builder.Services.AddTransient<PgAmortization, PgAmortizationViewModel>();
        builder.Services.AddTransient<PgExpense, PgExpenseViewModel>();
        builder.Services.AddTransient<PgAddExpense, PgAddExpenseViewModel>();
        builder.Services.AddTransient<CvSales, CvSalesViewModel>();
        builder.Services.AddTransient<PgAddCustomerDiscountType, PgAddCustomerDiscountTypeViewModel>();
        builder.Services.AddTransient<CvUsers, CvUsersViewModel>();
        builder.Services.AddTransient<PgSales, PgSalesViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
