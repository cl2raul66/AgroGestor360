using AgroGestor360.App.Services;
using AgroGestor360.App.ViewModels;
using AgroGestor360.App.ViewModels.Settings;
using AgroGestor360.App.ViewModels.Settings.BankAccounts;
using AgroGestor360.App.ViewModels.Settings.Products;
using AgroGestor360.App.ViewModels.Settings.Shareholders;
using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Settings;
using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.App.Views.Settings.Shareholders;
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

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<PgSignIn, PgSignInViewModel>();
        builder.Services.AddTransient<PgHome, PgHomeViewModel>();
        builder.Services.AddTransient<PgSettings, PgSettingsViewModel>();
        builder.Services.AddTransient<CvSeedCapital, CvSeedCapitalViewModel>();
        builder.Services.AddTransient<CvBankAccounts, CvBankAccountsViewModel>();
        builder.Services.AddTransient<PgAddAccountOrCard, PgAddAccountOrCardViewModel>();
        builder.Services.AddTransient<CvShareholders, CvShareholdersViewModel>();
        builder.Services.AddTransient<PgAddEditShareholder, PgAddEditShareholderViewModel>();
        builder.Services.AddTransient<CvProducts, CvProductsViewModel>();
        builder.Services.AddTransient<PgAddItem, PgAddItemViewModel>();
        builder.Services.AddTransient<PgAddProduct, PgAddProductViewModel>();
        builder.Services.AddTransient<PgCreateOffer, PgCreateOfferViewModel>();
        builder.Services.AddTransient<CvUsers, CvUsersViewModel>();
        builder.Services.AddTransient<PgExpense, PgExpenseViewModel>();
        builder.Services.AddTransient<PgLoans, PgLoansViewModel>();
        builder.Services.AddTransient<PgSales, PgSalesViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
