using AgroGestor360.App.Views.Sales;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using AgroGestor360.Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class PgSalesViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IProductsForSalesService productsForSalesServ;
    readonly ISellersService sellersServ;
    readonly ICustomersService customersServ;
    readonly IQuotesService quotationsServ;
    readonly IReportsService reportsServ;
    readonly IOrdersService ordersServ;
    readonly IAuthService authServ;
    readonly IInvoicesService invoicesServ;

    public PgSalesViewModel(IQuotesService quotesService, ISellersService sellersService, ICustomersService customersService, IProductsForSalesService productsForSalesService, IReportsService reportsService, IOrdersService ordersService, IAuthService authService, IInvoicesService invoicesService)
    {
        quotationsServ = quotesService;
        sellersServ = sellersService;
        customersServ = customersService;
        productsForSalesServ = productsForSalesService;
        reportsServ = reportsService;
        ordersServ = ordersService;
        authServ = authService;
        invoicesServ = invoicesService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);

        Task.Run(() =>
        {
            foreach (var f in Directory.GetFiles(FileSystem.CacheDirectory, "*.pdf"))
            {
                File.Delete(f);
            }
        });

        AppInfo = $"{Assembly.GetExecutingAssembly().GetName().Name} V.{VersionTracking.Default.CurrentVersion}";
    }

    [ObservableProperty]
    string? appInfo;

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
    async Task ViewBills()
    {
        IsBillsVisible = true;
        Invoices = new(await invoicesServ.GetAllAsync(serverURL));
    }

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    #region QUOTATION
    [ObservableProperty]
    ObservableCollection<DTO7>? quotations;

    [ObservableProperty]
    DTO7? selectedQuotation;

    [RelayCommand]
    async Task ShowQuotationDetail()
    {
        string bodyText = await GenerateBodyTextFromQuotation();
        await Shell.Current.DisplayAlert("Información", bodyText, "Cerrar");
        SelectedOrder = null;
        SelectedQuotation = null;
    }

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
        string[] options = ["Por rechazo del cliente.", "Por error del operador."];
        bool deletedInQuotations;

        var selectedOption = await Shell.Current.DisplayActionSheet("Seleccione el motivo de la eliminación", "Cancelar", null, options);

        switch (selectedOption)
        {
            case "Por rechazo del cliente.":
                deletedInQuotations = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = SelectedQuotation!.Code!, Status = QuotationStatus.Rejected });
                break;
            case "Por error del operador.":
                StringBuilder sb = new();
                sb.AppendLine($"¿Seguro que quiere eliminar la siguiente cotización?");
                sb.AppendLine("");
                sb.AppendLine($"No.: {SelectedQuotation!.Code}");
                sb.AppendLine($"Fecha de creación: {SelectedQuotation!.Date:dd MMM yyyy}");
                sb.AppendLine($"Vendedor: {SelectedQuotation!.SellerName}");
                sb.AppendLine($"Cliente: {SelectedQuotation!.CustomerName}");
                sb.AppendLine($"Total: {SelectedQuotation!.TotalAmount:N2}");
                sb.AppendLine("");
                sb.AppendLine("Inserte la contraseña:");
                var pwd = await Shell.Current.DisplayPromptAsync("Eliminar cotización", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
                if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
                {
                    SelectedQuotation = null;
                    SelectedOrder = null;
                    return;
                }

                var approved = await authServ.AuthRoot(serverURL, pwd);
                if (!approved)
                {
                    await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
                    SelectedQuotation = null;
                    SelectedOrder = null;
                    return;
                }

                deletedInQuotations = await quotationsServ.DeleteAsync(serverURL, SelectedQuotation!.Code!);
                break;
            default:
                SelectedQuotation = null;
                SelectedOrder = null;
                return;
        }

        if (deletedInQuotations)
        {
            Quotations!.Remove(SelectedQuotation);
        }
    }

    [RelayCommand]
    async Task ShareQuoteReport()
    {
        string file = GenerateFile();

        var wasGeneratePDF = await reportsServ.GeneratePDFCustomerQuoteReportAsync(serverURL, SelectedQuotation!.Code!, file);

        if (File.Exists(file) && wasGeneratePDF)
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Compartir cotización seleccionada",
                File = new ShareFile(file)
            });

            var result = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = SelectedQuotation!.Code!, Status = QuotationStatus.Sent });
            if (result)
            {
                int idx = Quotations!.IndexOf(SelectedQuotation);
                DTO7 dTO7 = new()
                {
                    Code = SelectedQuotation!.Code,
                    Date = SelectedQuotation!.Date,
                    SellerId = SelectedQuotation!.SellerId,
                    SellerName = SelectedQuotation!.SellerName,
                    CustomerId = SelectedQuotation!.CustomerId,
                    CustomerName = SelectedQuotation!.CustomerName,
                    TotalAmount = SelectedQuotation!.TotalAmount,
                    IsDraftStatus = false
                };
                Quotations[idx] = dTO7;
            }
        }
        SelectedOrder = null;
        SelectedQuotation = null;
    }

    [RelayCommand]
    async Task ViewQuoteReport()
    {
        string file = GenerateFile();

        bool wasGeneratePDF = await reportsServ.GeneratePDFCustomerQuoteReportAsync(serverURL, SelectedQuotation!.Code!, file);

        if (File.Exists(file) && wasGeneratePDF)
        {
            bool isOpen = await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(file)
            });

            if (isOpen)
            {
                var result = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = SelectedQuotation!.Code!, Status = QuotationStatus.Sent });
                if (result)
                {
                    int idx = Quotations!.IndexOf(SelectedQuotation);
                    DTO7 dTO7 = new()
                    {
                        Code = SelectedQuotation!.Code,
                        Date = SelectedQuotation!.Date,
                        SellerId = SelectedQuotation!.SellerId,
                        SellerName = SelectedQuotation!.SellerName,
                        CustomerId = SelectedQuotation!.CustomerId,
                        CustomerName = SelectedQuotation!.CustomerName,
                        TotalAmount = SelectedQuotation!.TotalAmount,
                        IsDraftStatus = false
                    };
                    Quotations[idx] = dTO7;
                }
            }
        }
        SelectedOrder = null;
        SelectedQuotation = null;
    }
    #endregion

    #region ORDERS
    bool ShowAddEditOrderState;

    [ObservableProperty]
    ObservableCollection<DTO8>? orders;

    [ObservableProperty]
    DTO8? selectedOrder;

    bool WaitPropertyChanged;

    [RelayCommand]
    async Task CreateOrderFomQuote()
    {
        var options = new List<string> { "Con cambios", "Sin cambios" };

        var selectedOption = await Shell.Current.DisplayActionSheet("Crear pedido desde la cotización", "Cancelar", null, [.. options]);

        switch (selectedOption)
        {
            case "Con cambios":
                ShowAddEditOrderState = true;
                string code = SelectedQuotation!.Code!;
                await ShowAddEditOrder();
                while (ShowAddEditOrderState)
                {
                    await Task.Delay(1000);
                }
                Orders ??= [];
                while (WaitPropertyChanged)
                {
                    await Task.Delay(1000);
                }
                break;
            case "Sin cambios":
                var result2 = await ordersServ.InsertFromQuoteAsync(serverURL, SelectedQuotation!);
                if (!string.IsNullOrEmpty(result2))
                {
                    bool resultChanges = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = SelectedQuotation!.Code!, Status = QuotationStatus.Accepted });
                    if (resultChanges)
                    {
                        _ = Quotations!.Remove(SelectedQuotation!);
                        Orders ??= [];
                        var orderGet = await ordersServ.GetByCodeAsync(serverURL, result2);
                        Orders.Insert(0, orderGet!);
                    }
                }
                else
                {
                    SelectedOrder = null;
                    SelectedQuotation = null;
                }
                break;
            default:
                SelectedOrder = null;
                SelectedQuotation = null;
                break;
        }
    }

    [RelayCommand]
    async Task ShowAddEditOrder()
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

        if (ShowAddEditOrderState)
        {
            DTO8_4 currentOrder = await ordersServ.GetDTO8_4FromQuotationAsync(serverURL, SelectedQuotation!) ?? new();
            sendData.Add("currentOrder", currentOrder);
        }

        await Shell.Current.GoToAsync(nameof(PgAddEditOrder), true, sendData);
    }

    [RelayCommand]
    async Task RemovedOrder()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el siguiente pedido?");
        sb.AppendLine("");
        sb.AppendLine($"No.: {SelectedOrder!.Code}");
        sb.AppendLine($"Fecha de creación: {SelectedOrder!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {SelectedOrder!.SellerName}");
        sb.AppendLine($"Cliente: {SelectedOrder!.CustomerName}");
        sb.AppendLine($"Total: {SelectedOrder!.TotalAmount:N2}");
        var pwd = await Shell.Current.DisplayAlert("Eliminar pedido", sb.ToString(), "Eliminar", "Cancelar");
        if (!pwd)
        {
            SelectedOrder = null;
            SelectedQuotation = null;
            return;
        }
        var result = await ordersServ.DeleteAsync(serverURL, SelectedOrder!.Code!);
        if (result)
        {
            Orders!.Remove(SelectedOrder);
        }
    }

    [RelayCommand]
    async Task ShowOrderDetail()
    {
        string bodyText = await GenerateBodyTextFromOrder();
        await Shell.Current.DisplayAlert("Información", bodyText, "Cerrar");
        SelectedOrder = null;
        SelectedQuotation = null;
    }
    #endregion

    #region BILLING
    [ObservableProperty]
    ObservableCollection<DTO10>? invoices;

    [ObservableProperty]
    DTO10? selectedInvoice;

    [RelayCommand]
    async Task ShowInvoiceDetail()
    {
        string bodyText = await GenerateBodyTextFromInvoice();
        await Shell.Current.DisplayAlert("Información", bodyText, "Cerrar");
        SelectedInvoice = null;
    }

    [RelayCommand]
    async Task ShowAddEditSale()
    {
        IsActive = true;
        var sellers = await sellersServ.GetAllAsync(serverURL);
        var customers = await customersServ.GetAllAsync(serverURL);
        var products = await productsForSalesServ.GetAllAsync(serverURL);
        var creditTime = await invoicesServ.GetCreditTimeAsync(serverURL);

        Dictionary<string, object> sendData = new()
        {
            { "sellers", sellers.ToArray() },
            { "customers", customers.ToArray()},
            { "products", products.ToArray() },
            { "creditTime", creditTime.ToArray() }
        };

        await Shell.Current.GoToAsync(nameof(PgAddEditSale), true, sendData);
    }

    [RelayCommand]
    async Task RemovedInvoice()
    {
        string[] options = ["Por rechazo del cliente.", "Por error del operador."];
        bool deletedInInvoice = false;

        var selectedOption = await Shell.Current.DisplayActionSheet("Seleccione el motivo de la eliminación", "Cancelar", null, options);

        switch (selectedOption)
        {
            case "Por rechazo del cliente.":
                DTO10_3 dTO = new() { Code = SelectedInvoice!.Code, Status = InvoiceStatus.Cancelled };
                deletedInInvoice = await invoicesServ.UpdateState(serverURL, dTO);
                break;
            case "Por error del operador.":
                //string invoiceStatus = SelectedInvoice!.Status switch
                //{
                //    InvoiceStatus.Cancelled => "Cancelada",
                //    InvoiceStatus.Paid => "Pagada",
                //    _ => "Pendiente"
                //};
                //StringBuilder sb = new();
                //sb.AppendLine($"¿Seguro que quiere eliminar la siguiente factura?");
                //sb.AppendLine("");
                //if (!string.IsNullOrEmpty(SelectedInvoice!.NumberFEL))
                //{
                //    sb.AppendLine($"Número FEL: {SelectedInvoice!.NumberFEL}");
                //}
                //sb.AppendLine($"No.: {SelectedInvoice!.Code}");
                //sb.AppendLine($"Estado de la factura: {invoiceStatus}");
                //sb.AppendLine($"Fecha de creación: {SelectedInvoice!.Date:dd MMM yyyy}");
                //sb.AppendLine($"Vendedor: {SelectedInvoice!.SellerName}");
                //sb.AppendLine($"Cliente: {SelectedInvoice!.CustomerName}");
                //sb.AppendLine($"Total: {SelectedInvoice!.TotalAmount:N2}");
                string bodyText = await GenerateBodyTextFromInvoice();
                bool result = await Shell.Current.DisplayAlert("Eliminar factura", bodyText, "Eliminar", "Cancelar");
                if (!result)
                {
                    SelectedOrder = null;
                    SelectedQuotation = null;
                    return;
                }

                deletedInInvoice = await invoicesServ.DeleteAsync(serverURL, SelectedInvoice!.Code!);
                break;
        }
        if (deletedInInvoice)
        {
            Invoices!.Remove(SelectedInvoice!);
        }
    }

    [RelayCommand]
    async Task ShowAmortizeInvoice()
    {
        IsActive = true;

        Dictionary<string, object> sendData = new()
        {
            { "currentInvoice", SelectedInvoice! }
        };

        await Shell.Current.GoToAsync(nameof(PgAmortizeInvoiceCredit), true, sendData);
    }
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
                r.Quotations ??= [];
                var quote = await quotationsServ.GetByCodeAsync(serverURL, result);
                r.Quotations.Insert(0, quote!);
            }
            r.SelectedOrder = null;
            r.SelectedQuotation = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO8_1, string>(this, "addorder", async (r, m) =>
        {
            IsActive = false;
            var result = await ordersServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Orders ??= [];
                var orderGet = await ordersServ.GetByCodeAsync(serverURL, result);
                r.Orders.Insert(0, orderGet!);
            }
            r.SelectedOrder = null;
            r.SelectedQuotation = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO8_1, string>(this, "addorderfromquote", async (r, m) =>
        {
            IsActive = false;
            var result = await ordersServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Orders ??= [];
                var orderGet = await ordersServ.GetByCodeAsync(serverURL, result);
                r.Orders.Insert(0, orderGet!);
            }

            bool result1 = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = m.Code, Status = QuotationStatus.Accepted });
            if (result1)
            {
                var currentQuotation = r.Quotations!.FirstOrDefault(x => x.Code == m.Code);
                _ = r.Quotations!.Remove(currentQuotation!);
            }

            r.SelectedOrder = null;
            r.SelectedQuotation = null;

            ShowAddEditOrderState = false;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO10_1, string>(this, "addinvoice", async (r, m) =>
        {
            IsActive = false;
            var result = await invoicesServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Invoices ??= [];
                var invoice = await invoicesServ.GetByCodeAsync(serverURL, result);
                r.Invoices.Insert(0, invoice!);
            }
            r.SelectedOrder = null;
            r.SelectedQuotation = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO10_2, string>(this, "setdepreciation", async (r, m) =>
        {
            IsActive = false;
            var result = await invoicesServ.DepreciationUpdate(serverURL, m);
            if (result)
            {
                r.Invoices ??= [];
                var invoice = await invoicesServ.GetByCodeAsync(serverURL, m.Code!);
                if (invoice is null)
                {
                    r.Invoices.Remove(r.SelectedInvoice!);
                }
                else
                {
                    var idx = r.Invoices.IndexOf(r.SelectedInvoice!);
                    r.Invoices[idx] = invoice;
                }
            }
            r.SelectedOrder = null;
            r.SelectedQuotation = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, string, string>(this, nameof(PgSalesViewModel), (r, m) =>
        {
            if (m == "cancel")
            {
                r.SelectedOrder = null;
                r.SelectedQuotation = null;
                r.SelectedInvoice = null;
                IsActive = false;
            }
            ShowAddEditOrderState = false;
        });
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Orders))
        {
            if (Orders is not null)
            {
                WaitPropertyChanged = false;
            }
        }
    }

    #region EXTRA
    public async void Initialize()
    {
        IsBusy = true;
        Quotations = new(await quotationsServ.GetAllAsync(serverURL));
        Orders = new(await ordersServ.GetAllAsync(serverURL));
        if (IsBillsVisible)
        {
            Invoices = new(await invoicesServ.GetAllAsync(serverURL));
        }
        IsBusy = false;
    }

    string GenerateFile()
    {
        string title = $"{SelectedQuotation!.Date:yyyyMMdd} - Cotización de {SelectedQuotation!.TotalAmount:F2} para {SelectedQuotation!.CustomerName}";

        string file = Path.Combine(FileSystem.CacheDirectory, title + ".pdf");

        return file;
    }

    async Task<string> GenerateBodyTextFromQuotation()
    {
        var currentQuotation = await quotationsServ.GetProductsByCodeAsync(serverURL, SelectedQuotation!.Code!);
        StringBuilder sb = new();
        sb.AppendLine($"No.: {currentQuotation!.Code}");
        sb.AppendLine($"Fecha de creación: {currentQuotation!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {currentQuotation!.SellerName}");
        sb.AppendLine($"Cliente: {currentQuotation!.CustomerName}");
        sb.AppendLine($"Estado: Pendiente");
        sb.AppendLine("Productos:");
        sb.AppendLine(string.Join(Environment.NewLine, currentQuotation.Products!));
        sb.AppendLine($"Total: {currentQuotation!.TotalAmount:N2}");
        return sb.ToString();
    }

    async Task<string> GenerateBodyTextFromOrder()
    {
        var currentOrder = await ordersServ.GetProductsByCodeAsync(serverURL, SelectedOrder!.Code!);
        StringBuilder sb = new();
        sb.AppendLine($"No.: {currentOrder!.Code}");
        sb.AppendLine($"Fecha de creación: {currentOrder!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {currentOrder!.SellerName}");
        sb.AppendLine($"Cliente: {currentOrder!.CustomerName}");
        sb.AppendLine($"Estado: Pendiente");
        sb.AppendLine("Productos:");
        sb.AppendLine(string.Join(Environment.NewLine, currentOrder.Products!));
        sb.AppendLine($"Total: {currentOrder!.TotalAmount:N2}");
        return sb.ToString();
    }

    async Task<string> GenerateBodyTextFromInvoice()
    {
        var currentInvoice = await invoicesServ.GetProductsByCodeAsync(serverURL, SelectedInvoice!.Code!);
        StringBuilder sb = new();
        sb.AppendLine($"No.: {currentInvoice!.Code}");
        sb.AppendLine($"Fecha de creación: {currentInvoice!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {currentInvoice!.SellerName}");
        sb.AppendLine($"Cliente: {currentInvoice!.CustomerName}");
        sb.AppendLine($"Estado: Pendiente");
        sb.AppendLine("Productos:");
        sb.AppendLine(string.Join(Environment.NewLine, currentInvoice.Products!));
        sb.AppendLine($"Total: {currentInvoice!.TotalAmount:N2}");
        return sb.ToString();
    }
    #endregion
}
