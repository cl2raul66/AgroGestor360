﻿using AgroGestor360.App.Services;
using AgroGestor360.App.View.Settings;
using AgroGestor360.App.ViewModels;
using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Expense;
using AgroGestor360.App.Views.Loans;
using AgroGestor360.App.Views.Sales;
using AgroGestor360.App.Views.Settings;
using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.App.Views.Settings.Connection;
using AgroGestor360.App.Views.Settings.Customers;
using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.App.Views.Settings.Shareholders;
using AgroGestor360.App.Views.Settings.Warehouse;
using AgroGestor360.Client.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

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

        builder.AddAppsettings();

        builder.Services.AddSingleton<IApiService>(serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var apiService = new ApiService();
            apiService.ConnectToHttpClient();
            apiService.SetClientAccessToken(configuration!["License:ClientAccessToken"]!);
            return apiService;
        });
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IReportsService, ReportsService>();
        builder.Services.AddSingleton<IOrganizationService, OrganizationService>();
        builder.Services.AddSingleton<IBankAccountsService, BankAccountsService>();
        builder.Services.AddSingleton<ISellersService, SellersService>();
        builder.Services.AddSingleton<ICustomersService, CustomersService>();
        builder.Services.AddSingleton<IMerchandiseService, MerchandiseService>();
        builder.Services.AddSingleton<IArticlesForWarehouseService, ArticlesForWarehouseService>();
        builder.Services.AddSingleton<IArticlesForSalesService, ArticlesForSalesService>();
        builder.Services.AddSingleton<IProductsForSalesService, ProductsForSalesService>();
        builder.Services.AddSingleton<IQuotesService, QuotesService>();
        builder.Services.AddSingleton<IFinancialInstrumentTypeService, FinancialInstrumentTypeService>();
        builder.Services.AddSingleton<IMeasurementService, MeasurementService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<PgSignIn, PgSignInViewModel>();
        builder.Services.AddTransient<PgHome, PgHomeViewModel>();
        builder.Services.AddTransient<PgSettings, PgSettingsViewModel>();
        builder.Services.AddTransient<CvConnection, CvConnectionViewModel>();
        builder.Services.AddTransient<PgSetURL, PgSetURLViewModel>();
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
        builder.Services.AddTransient<PgAddEditWarehouse, PgAddEditWarehouseViewModel>();
        builder.Services.AddTransient<PgLoans, PgLoansViewModel>();
        builder.Services.AddTransient<PgAddLoan, PgAddLoanViewModel>();
        builder.Services.AddTransient<PgAmortization, PgAmortizationViewModel>();
        builder.Services.AddTransient<PgExpense, PgExpenseViewModel>();
        builder.Services.AddTransient<PgAddExpense, PgAddExpenseViewModel>();
        builder.Services.AddTransient<CvUsers, CvUsersViewModel>();
        builder.Services.AddTransient<PgSales, PgSalesViewModel>();
        builder.Services.AddTransient<CvSellers, CvSellersViewModel>();
        builder.Services.AddTransient<PgAddEditSeller, PgAddEditSellerViewModel>();
        builder.Services.AddTransient<PgAddEditQuote, PgAddEditQuoteViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void AddAppsettings(this MauiAppBuilder builder)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.appsettings.json")!;
        if (stream is not null)
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
            builder.Configuration.AddConfiguration(config);
        }
    }
}
