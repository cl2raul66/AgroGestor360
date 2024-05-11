using AgroGestor360.App.Views.Sales;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace AgroGestor360.App.ViewModels;

public partial class PgSalesViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IProductsForSalesService productsForSalesServ; 
    readonly ISellersService sellersServ;
    readonly ICustomersService customersServ;
    readonly IQuotesService quotationsServ;
    readonly IReportsService reportsServ;

    public PgSalesViewModel(IQuotesService quotesService, ISellersService sellersService, ICustomersService customersService, IProductsForSalesService productsForSalesService, IReportsService reportsService)
    {        
        quotationsServ = quotesService;
        sellersServ = sellersService;
        customersServ = customersService;
        productsForSalesServ = productsForSalesService;
        reportsServ = reportsService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);

        Task.Run(() =>
        {
            foreach (var f in Directory.GetFiles(FileSystem.CacheDirectory, "*.pdf"))
            {
                File.Delete(f);
            }
        });
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    bool isBillsVisible;

    [RelayCommand]
    void ViewPresale()
    {
        IsBillsVisible = false;
    }

    [RelayCommand]
    void ViewBills()
    {
        IsBillsVisible = true;
    }

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    #region QUOTATION
    [ObservableProperty]
    ObservableCollection<DTO7>? quotations;

    [ObservableProperty]
    DTO7? selectedQuotation;

    [RelayCommand]
    async Task ShowAddEditQuote()
    {
        IsActive = true;
        var sellers = await sellersServ.GetAllAsync(serverURL);
        var customers = await customersServ.GetAllAsync(serverURL);
        var products = await productsForSalesServ.GetAllAsync(serverURL);
        Dictionary<string, object> sendData = new()
        {
            { "sellers", sellers.ToList() },
            { "customers", customers.ToList()},
            { "products", products.ToList() }
        };
        await Shell.Current.GoToAsync(nameof(PgAddEditQuote), true, sendData);
    }

    [RelayCommand]
    async Task RemovedQuote()
    {
        var result = await quotationsServ.DeleteAsync(serverURL, SelectedQuotation!.Code!);
        if (result)
        {
            Quotations!.Remove(SelectedQuotation);
        }
    }

    [RelayCommand]
    async Task ShareQuoteReport()
    {
        string file = GenerateFile();

        var result = await reportsServ.GeneratePDFCustomerQuoteReportAsync(serverURL, SelectedQuotation!.Code!, file);

        if (File.Exists(file) && result)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Compartir cotización seleccionada",
                File = new ShareFile(file)
            });
        }
    }

    [RelayCommand]
    async Task ViewQuoteReport()
    {
        string file = GenerateFile();

        var result = await reportsServ.GeneratePDFCustomerQuoteReportAsync(serverURL, SelectedQuotation!.Code!, file);

        if (File.Exists(file) && result)
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(file)
            });
        }
    }
    #endregion

    #region ORDERS

    #endregion

    #region BILLING

    #endregion

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO7_1, string>(this, "addquote", async (r, m) =>
        {
            IsActive = false;
            var result = await quotationsServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                Quotations ??= [];
                var quote = await quotationsServ.GetByIdAsync(serverURL, result);
                Quotations.Insert(0, quote!);
            }
            SelectedQuotation = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, string, string>(this, nameof(PgAddEditQuote), (r, m) =>
        {
            if (m == "cancel")
            {
                r.SelectedQuotation = null;
                IsActive = false;
            }
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        IsBusy = true;
        Quotations = new(await quotationsServ.GetAllAsync(serverURL));
        IsBusy = false;
    }

    string GenerateFile()
    {
        string title = $"{SelectedQuotation!.QuotationDate:yyyyMMdd} - Cotización de {SelectedQuotation!.TotalAmount:F2} para {SelectedQuotation!.CustomerName}";

        string file = Path.Combine(FileSystem.CacheDirectory, title + ".pdf");

        return file;        
    }
    #endregion
}
