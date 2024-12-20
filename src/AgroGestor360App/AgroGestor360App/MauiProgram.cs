using Syncfusion.Maui.Core.Hosting;
using AgroGestor360App.Services;
using AgroGestor360App.Tools;
using AgroGestor360App.ViewModels;
using AgroGestor360App.Views;
using AgroGestor360App.Views.Expense;
using AgroGestor360App.Views.Loans;
using AgroGestor360App.Views.Sales;
using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Services;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AgroGestor360App.Views.Dialogs;
using System.Reflection;

namespace AgroGestor360App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        //builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionCore()
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
            apiService.SetClientDevicePlatform(CurrentDevicePlatform.GetDevicePlatform());
            apiService.SetClientAccessToken(configuration!["License:ClientAccessToken"]!);
            return apiService;
        });
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IReportsService, ReportsService>();
        builder.Services.AddSingleton<IOrganizationService, OrganizationService>();
        builder.Services.AddSingleton<IBankAccountsService, BankAccountsService>();
        builder.Services.AddSingleton<IDiscountsCustomersService, DiscountsCustomersService>();
        builder.Services.AddSingleton<ILineCreditsService, LineCreditsService>();
        builder.Services.AddSingleton<ITimeLimitsCreditsService, TimeLimitsCreditsService>();
        builder.Services.AddSingleton<ICustomersService, CustomersService>();
        builder.Services.AddSingleton<ISellersService, SellersService>();
        builder.Services.AddSingleton<IMerchandiseService, MerchandiseService>();
        builder.Services.AddSingleton<IArticlesForWarehouseService, ArticlesForWarehouseService>();
        builder.Services.AddSingleton<IArticlesForSalesService, ArticlesForSalesService>();
        builder.Services.AddSingleton<IProductsForSalesService, ProductsForSalesService>();
        builder.Services.AddSingleton<IQuotesService, QuotesService>();
        builder.Services.AddSingleton<IOrdersService, OrdersService>();
        builder.Services.AddSingleton<ISaleRecordsService, SaleRecordsService>();
        builder.Services.AddSingleton<IFinancialInstrumentTypeService, FinancialInstrumentTypeService>();
        builder.Services.AddSingleton<IPaymentTypeService, PaymentTypeService>();
        builder.Services.AddSingleton<IMeasurementService, MeasurementService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IReconciliationService, ReconciliationService>();
        builder.Services.AddSingleton<ITypeFrequencyReconciliationPolicyService, TypeFrequencyReconciliationPolicyService>();

        builder.Services.AddTransient<PgSignIn, PgSignInViewModel>();
        builder.Services.AddTransient<PgAuthenticationDialog, PgAuthenticationDialogViewModel>();
        builder.Services.AddTransient<PgSelectDiscountsOptionsDialog, PgSelectDiscountsOptionsDialogViewModel>();
        builder.Services.AddTransient<PgHome, PgHomeViewModel>();
        builder.Services.AddTransient<PgSettings, PgSettingsViewModel>();
        builder.Services.AddTransient<CvConnection, CvConnectionViewModel>();
        builder.Services.AddTransient<PgSetURL, PgSetURLViewModel>();
        builder.Services.AddTransient<CvBankAccounts, CvBankAccountsViewModel>();
        builder.Services.AddTransient<PgAddAccountOrCard, PgAddAccountOrCardViewModel>();
        builder.Services.AddTransient<CvDiscounts, CvDiscountsViewModel>();
        builder.Services.AddTransient<PgAddDiscountForCustomer, PgAddDiscountForCustomerViewModel>();
        builder.Services.AddTransient<CvLineCredits, CvLineCreditsViewModel>();
        builder.Services.AddTransient<PgAddLineCredit, PgAddLineCreditViewModel>();
        builder.Services.AddTransient<PgAdminTimeLimit, PgAdminTimeLimitViewModel>();
        builder.Services.AddTransient<PgSetDefaultTimeLimit, PgSetDefaultTimeLimitViewModel>();
        builder.Services.AddTransient<PgSetCreditForCustomer, PgSetCreditForCustomerViewModel>();
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
        builder.Services.AddTransient<CvSales, CvSalesViewModel>();
        builder.Services.AddTransient<PgAddEditQuote, PgAddEditQuoteViewModel>();
        builder.Services.AddTransient<PgAddEditOrder, PgAddEditOrderViewModel>();
        builder.Services.AddTransient<PgAddEditSale, PgAddEditSaleViewModel>();
        builder.Services.AddTransient<PgDeletedInvoice, PgDeletedInvoiceViewModel>();
        builder.Services.AddTransient<PgDeletedInSale, PgDeletedInSaleViewModel>();
        builder.Services.AddTransient<PgTotalOrPartialPayment, PgTotalOrPartialPaymentViewModel>();
        builder.Services.AddTransient<PgReports, PgReportsViewModel>();

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
