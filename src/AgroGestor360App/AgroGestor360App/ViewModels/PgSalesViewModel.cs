﻿// Ignore Spelling: auth api

using AgroGestor360App.Views.Sales;
using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using AgroGestor360Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace AgroGestor360App.ViewModels;

[QueryProperty(nameof(IsShowAddQuote), "ShowAddQuote")]
[QueryProperty(nameof(IsShowAddOrder), "ShowAddOrder")]
[QueryProperty(nameof(IsShowAddSale), "ShowAddSale")]
public partial class PgSalesViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IProductsForSalesService productsForSalesServ;
    readonly ISellersService sellersServ;
    readonly ICustomersService customersServ;
    readonly IQuotesService quotationsServ;
    readonly IReportsService reportsServ;
    readonly IOrdersService ordersServ;
    // readonly IAuthService authServ;
    readonly ISaleRecordsService salesServ;
    readonly IApiService apiServ;
    readonly ITimeLimitsCreditsService timeLimitsCreditsServ;
    readonly IPaymentTypeService paymentTypeServ;
    //readonly SemaphoreSlim semaphore = new(0, 2);

    public PgSalesViewModel(IQuotesService quotesService, ISellersService sellersService, ICustomersService customersService, IProductsForSalesService productsForSalesService, IReportsService reportsService, IOrdersService ordersService, ISaleRecordsService salesService, IApiService apiService, ITimeLimitsCreditsService timeLimitsCreditsService, IPaymentTypeService paymentTypeService)
    {
        quotationsServ = quotesService;
        sellersServ = sellersService;
        customersServ = customersService;
        productsForSalesServ = productsForSalesService;
        reportsServ = reportsService;
        ordersServ = ordersService;
        //authServ = authService;
        salesServ = salesService;
        apiServ = apiService;
        timeLimitsCreditsServ = timeLimitsCreditsService;
        paymentTypeServ = paymentTypeService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);

        apiServ.OnReceiveStatusMessage += ApiServ_OnReceiveStatusMessage;

        Task.Run(() =>
        {
            foreach (var f in Directory.GetFiles(FileSystem.CacheDirectory, "*.pdf"))
            {
                File.Delete(f);
            }
        });

        AppInfo = $"Versión: {VersionTracking.Default.CurrentVersion}";
    }

    [ObservableProperty]
    bool isShowAddQuote;

    [ObservableProperty]
    bool isShowAddOrder;

    [ObservableProperty]
    bool isShowAddSale;

    [ObservableProperty]
    bool haveConnection;

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
        IsActive = true;
        Dictionary<string, object> sendData = new()
        {
            { "dialog", "deletedquote" }
        };
        await Shell.Current.GoToAsync(nameof(PgDeletedInSale), true, sendData);
    }

    [RelayCommand]
    async Task ShareQuoteReport()
    {
        string file = GeneratePathFileForQuotation();

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
        string file = GeneratePathFileForQuotation();

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
    [ObservableProperty]
    ObservableCollection<DTO8>? orders;

    [ObservableProperty]
    DTO8? selectedOrder;

    [RelayCommand]
    async Task CreateOrderFomQuote()
    {
        await ShowSalesConfirmationDialog("Crear un pedido desde la cotización");

        if (ResultSalesConfirmationDialog.HasValue && ResultSalesConfirmationDialog.Value)
        {
            if (OptionSalesConfirmationDialog)
            {
                DTO7 currentQuote = Quotations!.First(x => x.Code == SelectedQuotation!.Code);
                var resultInsert = await ordersServ.InsertFromQuoteAsync(serverURL, currentQuote);
                if (!string.IsNullOrEmpty(resultInsert))
                {
                    bool resultChanges = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = currentQuote.Code!, Status = QuotationStatus.Accepted });
                    if (resultChanges)
                    {
                        _ = Quotations!.Remove(currentQuote);
                        Orders ??= [];
                        var orderGet = await ordersServ.GetByCodeAsync(serverURL, resultInsert);
                        Orders.Insert(0, orderGet!);
                    }
                }
            }
            else
            {
                //semaphore.Wait();
                //semaphore.Wait();
                await ShowAddEditOrder();
                Orders ??= [];
            }
        }

        SelectedOrder = null;
        SelectedQuotation = null;
        ResultSalesConfirmationDialog = null;
    }

    [RelayCommand]
    async Task ShowAddEditOrder()
    {
        IsActive = true;

        var sellersTask = sellersServ.GetAllAsync(serverURL);
        var customersTask = customersServ.GetAllAsync(serverURL);
        var productsTask = productsForSalesServ.GetAllAsync(serverURL);

        await Task.WhenAll(sellersTask, customersTask, productsTask);

        DTO6[] sellers = [.. sellersTask.Result];
        DTO5_1[] customers = [.. customersTask.Result];
        DTO4[] products = [.. productsTask.Result];

        Dictionary<string, object> sendData = new()
        {
            { "sellers", sellers.ToList() },
            { "customers", customers.ToList()},
            { "products", products.ToList() }
        };

        if (ResultSalesConfirmationDialog.HasValue && ResultSalesConfirmationDialog.Value)
        {
            DTO_SB1 currentOrder = await ordersServ.GetDTO_SB1FromQuotationAsync(serverURL, SelectedQuotation!.Code!) ?? new();
            sendData.Add("currentOrder", currentOrder);
        }

        await Shell.Current.GoToAsync(nameof(PgAddEditOrder), true, sendData);
    }

    [RelayCommand]
    async Task RemovedOrder()
    {
        IsActive = true;
        Dictionary<string, object> sendData = new()
        {
            { "dialog", "deletedorder" }
        };
        await Shell.Current.GoToAsync(nameof(PgDeletedInSale), true, sendData);
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

    [ObservableProperty]
    bool onCredit;

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

        var sellersTask = sellersServ.GetAllAsync(serverURL);
        var customersTask = customersServ.GetAllAsync(serverURL);
        var productsTask = productsForSalesServ.GetAllAsync(serverURL);
        var creditTimeTask = timeLimitsCreditsServ.GetAllAsync(serverURL);
        var defaultCreditTimeTask = timeLimitsCreditsServ.GetDefaultAsync(serverURL);

        await Task.WhenAll(sellersTask, customersTask, productsTask, creditTimeTask, defaultCreditTimeTask);

        var sellers = sellersTask.Result;
        var customers = customersTask.Result;
        var products = productsTask.Result;
        var creditTime = creditTimeTask.Result;
        var defaultCreditTime = defaultCreditTimeTask.Result;

        Dictionary<string, object> sendData = new()
        {
            { "sellers", sellers.ToArray() },
            { "customers", customers.ToArray()},
            { "products", products.ToArray() },
            { "creditTime", creditTime.ToArray() },
            { "defaultcredittime", defaultCreditTime! }
        };

        if (ResultSalesConfirmationDialog.HasValue && ResultSalesConfirmationDialog.Value)
        {
            DTO_SB1 currentInvoice = await salesServ.GetDTO_SB1FromOrderAsync(serverURL, SelectedOrder!.Code!) ?? new();
            sendData.Add("currentInvoice", currentInvoice);
        }

        await Shell.Current.GoToAsync(nameof(PgAddEditSale), true, sendData);
    }

    [RelayCommand]
    async Task RemovedInvoice()
    {
        IsActive = true;
        var concepts = await salesServ.GetConceptsAsync(serverURL);
        Dictionary<string, object> sendData = new()
        {
            { "concepts", concepts.ToArray() }
        };
        await Shell.Current.GoToAsync(nameof(PgDeletedInvoice), true, sendData);
    }

    [RelayCommand]
    async Task ShowAmortizeInvoice()
    {
        IsActive = true;

        Dictionary<string, object> sendData = new()
        {
            {"SendToken", "E7F8G9H0-I1J2-3K4L-M5N6-O7P8Q9R0S1T2"},
            { "CurrentSale", SelectedInvoice! }
        };

        await Shell.Current.GoToAsync(nameof(PgTotalOrPartialPayment), true, sendData);
    }

    [RelayCommand]
    async Task CompletePayment()
    {
        IsActive = true;

        Dictionary<string, object> sendData = new()
        {
            {"SendToken", "F4E5D6C7-B8A9-0B1C-D2E3-F4567890ABCD"},
            {"CurrentSale", SelectedInvoice!}
        };

        await Shell.Current.GoToAsync(nameof(PgTotalOrPartialPayment), true, sendData);
    }

    [RelayCommand]
    async Task CreateInvoiceFromQuote()
    {
        DTO10_2 data = new() { Code = SelectedQuotation!.Code };

        var resultInsert = await salesServ.InsertFromQuoteAsync(serverURL, data);
        if (!string.IsNullOrEmpty(resultInsert))
        {
            bool resultChanges = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = SelectedQuotation!.Code!, Status = QuotationStatus.Accepted });
            if (resultChanges)
            {
                _ = Quotations!.Remove(SelectedQuotation!);
                Orders ??= [];
                var orderGet = await ordersServ.GetByCodeAsync(serverURL, resultInsert);
                Orders.Insert(0, orderGet!);
            }
        }
        else
        {
            SelectedOrder = null;
            SelectedQuotation = null;
        }

        SelectedOrder = null;
        SelectedQuotation = null;
        ViewBills();
    }

    [RelayCommand]
    async Task CreateInvoiceFromOrder()
    {
        await ShowSalesConfirmationDialog("Crear una venta desde el pedido");
        if (ResultSalesConfirmationDialog.HasValue && ResultSalesConfirmationDialog.Value)
        {
            if (OptionSalesConfirmationDialog)
            {
                DTO8 currentOrder = Orders!.First(x => x.Code == SelectedOrder!.Code);
                var resultInsert = await salesServ.InsertFromOrderAsync(serverURL, currentOrder);
                if (!string.IsNullOrEmpty(resultInsert))
                {
                    bool resultChanges = await ordersServ.ChangeByStatusAsync(serverURL, new DTO8_6() { Code = currentOrder.Code!, Status = OrderStatus.Completed });
                    if (resultChanges)
                    {
                        _ = Orders!.Remove(currentOrder);
                        Invoices ??= [];
                        var orderGet = await salesServ.GetByCodeAsync(serverURL, resultInsert);
                        Invoices.Insert(0, orderGet!);
                        ViewBills();
                    }
                }
            }
            else
            {
                ViewBills();
                await ShowAddEditSale();
                //semaphore.Wait();
                Invoices ??= [];
                //semaphore.Wait();  
            }
            ResultSalesConfirmationDialog = null;
        }
        SelectedOrder = null;
        SelectedQuotation = null;
    }
    #endregion    

    #region DIALOGUE FOR TRANSITION CONFIRMATION WITHIN SALES
    [ObservableProperty]
    bool isVisibleSalesConfirmationDialog;

    [ObservableProperty]
    string? titleSalesConfirmationDialog;

    /// <summary>
    /// Se usa para registrar la opción seleccionada.
    /// <code>
    /// True = Sin cambios
    /// False = Con cambios
    /// </code>
    /// </summary>
    [ObservableProperty]
    bool optionSalesConfirmationDialog;

    /// <summary>
    /// Se usa para registrar si se presionó el botón Confirmar o Cancelar.
    /// <code>
    /// True = Confirmar
    /// False = Cancelar
    /// </code>
    /// </summary>
    [ObservableProperty]
    bool? resultSalesConfirmationDialog;

    [RelayCommand]
    void SetSalesConfirmationDialog()
    {
        ResultSalesConfirmationDialog = true;
        TitleSalesConfirmationDialog = null;
        IsVisibleSalesConfirmationDialog = false;
    }

    [RelayCommand]
    void CancelSalesConfirmationDialog()
    {
        ResultSalesConfirmationDialog = false;
        TitleSalesConfirmationDialog = null;
        IsVisibleSalesConfirmationDialog = false;
    }

    async Task ShowSalesConfirmationDialog(string title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            TitleSalesConfirmationDialog = title;
            IsVisibleSalesConfirmationDialog = true;
            OptionSalesConfirmationDialog = true;

            while (IsVisibleSalesConfirmationDialog)
            {
                await Task.Delay(1000);
            }
        }
    }
    #endregion
       
    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }

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

            bool resultChanges = await quotationsServ.ChangesByStatusAsync(serverURL, new() { Code = m.Code, Status = QuotationStatus.Accepted });
            if (resultChanges)
            {
                var currentQuotation = r.Quotations!.FirstOrDefault(x => x.Code == m.Code);
                _ = r.Quotations!.Remove(currentQuotation!);
            }

            r.SelectedOrder = null;
            r.SelectedQuotation = null;

            //semaphore.Release();
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO10_1, string>(this, "addinvoice", async (r, m) =>
        {
            IsActive = false;
            var result = await salesServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Invoices ??= [];
                var invoiceGet = await salesServ.GetByCodeAsync(serverURL, result);
                r.Invoices.Insert(0, invoiceGet!);
            }
            r.SelectedInvoice = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO10_1, string>(this, "addinvoicefromorder", async (r, m) =>
        {
            IsActive = false;
            var result = await salesServ.InsertFromOrderWithModificationsAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Invoices ??= [];
                var invoiceGet = await salesServ.GetByCodeAsync(serverURL, result);
                r.Invoices.Insert(0, invoiceGet!);
            }

            bool resultChanges = await ordersServ.ChangeByStatusAsync(serverURL, new() { Code = m.Code, Status = OrderStatus.Completed });
            if (resultChanges)
            {
                var currentOrder = r.Orders!.FirstOrDefault(x => x.Code == m.Code);
                _ = r.Orders!.Remove(currentOrder!);
            }

            r.SelectedOrder = null;
            r.SelectedQuotation = null;

            //semaphore.Release();
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO10_2, string>(this, "setdepreciation", async (r, m) =>
        {
            IsActive = false;
            var result = await salesServ.RepaymentAsync(serverURL, m);
            if (result)
            {
                r.Invoices ??= [];
                var invoice = await salesServ.GetByCodeAsync(serverURL, m.Code!);
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

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, string, string>(this, "deletedquote", async (r, m) =>
        {
            IsActive = false;
            DTO7 currentQuote = Quotations!.First(x => x.Code == SelectedQuotation!.Code);
            bool deletedQuote = false;

            if (bool.Parse(m))
            {
                //IsVisiblePwdDialog = true;
                //while (IsVisiblePwdDialog)
                //{
                //    await Task.Delay(100);
                //}
                //deletedQuote = ResultPWD && await quotationsServ.DeleteAsync(serverURL, currentQuote.Code!);
            }
            else
            {
                DTO7_3 dTO = new() { Code = currentQuote.Code, Status = QuotationStatus.Rejected };
                deletedQuote = await quotationsServ.ChangesByStatusAsync(serverURL, dTO);
            }

            if (deletedQuote)
            {
                Quotations!.Remove(currentQuote);
            }

            SelectedOrder = null;
            SelectedQuotation = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, string, string>(this, "deletedorder", async (r, m) =>
        {
            IsActive = false;
            bool deletedOrder = false;
            DTO8 currentOrder = Orders!.First(x => x.Code == SelectedOrder!.Code);

            if (bool.Parse(m))
            {
                //IsVisiblePwdDialog = true;
                //while (IsVisiblePwdDialog)
                //{
                //    await Task.Delay(100);
                //}
                //deletedOrder = ResultPWD && await ordersServ.DeleteAsync(serverURL, currentOrder.Code!);
            }
            else
            {
                DTO8_6 dTO = new() { Code = currentOrder.Code, Status = OrderStatus.Rejected };
                deletedOrder = await ordersServ.ChangeByStatusAsync(serverURL, dTO);
            }

            if (deletedOrder)
            {
                Orders!.Remove(currentOrder);
            }

            SelectedOrder = null;
            SelectedQuotation = null;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, ConceptForDeletedSaleRecord, string>(this, "deletedinvoice", async (r, m) =>
        {
            IsActive = false;
            bool isEmpty = ConceptForDeletedInvoiceIsEmpty(m);
            bool deletedInInvoice = false;
            DTO10 currentInvoice = Invoices!.First(x => x.Code == SelectedInvoice!.Code);

            if (isEmpty)
            {
                //IsVisiblePwdDialog = true;
                ////while (IsVisiblePwdDialog)
                ////{
                ////    await Task.Delay(100);
                ////}
                //deletedInInvoice = ResultPWD && await salesServ.DeleteAsync(serverURL, currentInvoice.Code!);
            }
            else
            {
                DTO10_3 dTO = new() { Code = currentInvoice.Code, Status = SaleStatus.Cancelled, Notes = m.Concept };
                deletedInInvoice = await salesServ.ChangeByStatusAsync(serverURL, dTO);
            }

            if (deletedInInvoice)
            {
                Invoices!.Remove(currentInvoice);
            }

            SelectedInvoice = null;
        });

        //Abono a registro de venta
        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO10_2, string>(this, "E7F8G9H0-I1J2-3K4L-M5N6-O7P8Q9R0S1T2", async (r, m) =>
        {
            bool result = await salesServ.RepaymentAsync(serverURL, m);
            if (result)
            {
                int idx = Invoices!.IndexOf(SelectedInvoice!);
                var updateInvoice = await salesServ.GetByCodeAsync(serverURL, SelectedInvoice!.Code!);
                if (updateInvoice is not null)
                {
                    Invoices[idx] = updateInvoice;
                }
            }

            SelectedInvoice = null;
            IsActive = false;
        });

        //Pago total del registro de venta
        WeakReferenceMessenger.Default.Register<PgSalesViewModel, DTO10_2, string>(this, "F4E5D6C7-B8A9-0B1C-D2E3-F4567890ABCD", async (r, m) =>
        {
            bool result = await salesServ.RepaymentAsync(serverURL, m);
            if (result)
            {
                Invoices!.Remove(SelectedInvoice!);
            }

            SelectedInvoice = null;
            IsActive = false;
        });

        WeakReferenceMessenger.Default.Register<PgSalesViewModel, string, string>(this, "A1B2C3D4-E5F6-7890-ABCD-EF1234567890", (r, m) =>
        {
            if (m == "cancel")
            {
                SelectedQuotation = null;
                SelectedOrder = null;
                SelectedInvoice = null;
            }
            IsActive = false;
        });
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Orders))
        {
            if (Orders is not null)
            {
                //semaphore.Release();
            }
        }

        if (e.PropertyName == nameof(IsBillsVisible))
        {
            Invoices = IsBillsVisible ? new(await salesServ.GetAllAsync(serverURL)) : null;
        }

        if (e.PropertyName == nameof(IsShowAddOrder))
        {
            if (IsShowAddOrder)
            {
                await ShowAddEditOrder();
            }
        }

        if (e.PropertyName == nameof(IsShowAddQuote))
        {
            if (IsShowAddQuote)
            {
                await ShowAddEditQuote();
            }
        }

        if (e.PropertyName == nameof(IsShowAddSale))
        {
            if (IsShowAddSale)
            {
                ViewBills();
                await ShowAddEditSale();
            }
        }

        //if (e.PropertyName == nameof(IsOpenCompletePayment))
        //{
        //    if (IsOpenCompletePayment)
        //    {
        //        PaymentsTypes = [.. paymentTypeServ.GetAll()];
        //    }
        //}
    }

    public async void Initialize()
    {
        HaveConnection = await apiServ.ConnectToServerHub(serverURL);

        IsBusy = true;
        var getAllQuotationsTask = quotationsServ.GetAllAsync(serverURL);
        var getAllOrdersTask = ordersServ.GetAllAsync(serverURL);

        await Task.WhenAll(getAllQuotationsTask, getAllOrdersTask);
        Quotations = new ObservableCollection<DTO7>(getAllQuotationsTask.Result);
        Orders = new ObservableCollection<DTO8>(getAllOrdersTask.Result);

        //if (IsBillsVisible)
        //{
        //    Invoices = new(await salesServ.GetAllAsync(serverURL));
        //}
        IsBusy = false;
    }

    #region EXTRA
    string GeneratePathFileForQuotation()
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
        if (string.IsNullOrEmpty(currentQuotation!.OrganizationName))
        {
            sb.AppendLine($"Cliente: {currentQuotation!.CustomerName}");
        }
        else
        {
            sb.AppendLine($"Entidad: {currentQuotation!.OrganizationName}");
            sb.AppendLine($"Contacto: {currentQuotation!.CustomerName}");
        }
        sb.AppendLine($"Estado: Pendiente");
        sb.AppendLine("Productos:");
        sb.AppendLine(string.Join(Environment.NewLine, currentQuotation.Products!));
        sb.AppendLine($"Total: {currentQuotation!.TotalAmount:C}");
        return sb.ToString();
    }

    async Task<string> GenerateBodyTextFromOrder()
    {
        var currentOrder = await ordersServ.GetProductsByCodeAsync(serverURL, SelectedOrder!.Code!);
        StringBuilder sb = new();
        sb.AppendLine($"No.: {currentOrder!.Code}");
        sb.AppendLine($"Fecha de creación: {currentOrder!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {currentOrder!.SellerName}");
        if (string.IsNullOrEmpty(currentOrder!.OrganizationName))
        {
            sb.AppendLine($"Cliente: {currentOrder!.CustomerName}");
        }
        else
        {
            sb.AppendLine($"Entidad: {currentOrder!.OrganizationName}");
            sb.AppendLine($"Contacto: {currentOrder!.CustomerName}");
        }
        sb.AppendLine($"Estado: Pendiente");
        sb.AppendLine("Productos:");
        sb.AppendLine(string.Join(Environment.NewLine, currentOrder.Products!));
        sb.AppendLine($"Total: {currentOrder!.TotalAmount:C}");
        return sb.ToString();
    }

    async Task<string> GenerateBodyTextFromInvoice()
    {
        var currentInvoice = await salesServ.GetProductsByCodeAsync(serverURL, SelectedInvoice!.Code!);
        StringBuilder sb = new();
        sb.AppendLine($"No.: {currentInvoice!.Code}");
        sb.AppendLine($"Fecha de creación: {currentInvoice!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {currentInvoice!.SellerName}");
        if (string.IsNullOrEmpty(currentInvoice!.OrganizationName))
        {
            sb.AppendLine($"Cliente: {currentInvoice!.CustomerName}");
        }
        else
        {
            sb.AppendLine($"Entidad: {currentInvoice!.OrganizationName}");
            sb.AppendLine($"Contacto: {currentInvoice!.CustomerName}");
        }
        sb.AppendLine($"Estado: Pendiente");
        sb.AppendLine("Productos:");
        sb.AppendLine(string.Join(Environment.NewLine, currentInvoice.Products!));
        sb.AppendLine($"Total: {currentInvoice!.TotalAmount:C}");
        return sb.ToString();
    }

    private async Task<string> DisplayActionSheetForRemoval()
    {
        string[] options = ["Por rechazo del cliente.", "Por error del operador."];
        var selectedOption = await Shell.Current.DisplayActionSheet("Seleccione el motivo de la eliminación", "Cancelar", null, options);
        return selectedOption;
    }

    private string GenerateConfirmationMessageForQuote()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar la siguiente cotización?");
        sb.AppendLine("");
        sb.AppendLine($"No.: {SelectedQuotation!.Code}");
        sb.AppendLine($"Fecha de creación: {SelectedQuotation!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {SelectedQuotation!.SellerName}");
        sb.AppendLine($"Cliente: {SelectedQuotation!.CustomerName}");
        sb.AppendLine($"Total: {SelectedQuotation!.TotalAmount:C}");
        return sb.ToString();
    }

    private string GenerateConfirmationMessageForOrder()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el siguiente pedido?");
        sb.AppendLine("");
        sb.AppendLine($"No.: {SelectedOrder!.Code}");
        sb.AppendLine($"Fecha de creación: {SelectedOrder!.Date:dd MMM yyyy}");
        sb.AppendLine($"Vendedor: {SelectedOrder!.SellerName}");
        sb.AppendLine($"Cliente: {SelectedOrder!.CustomerName}");
        sb.AppendLine($"Total: {SelectedOrder!.TotalAmount:C}");
        return sb.ToString();
    }

    async Task<bool> ConfirmRemoval(string message, bool withAuthorize = false)
    {
        if (withAuthorize)
        {
            var pwd = await Shell.Current.DisplayPromptAsync("Eliminar", "Inserte la contraseña:", "Autenticar y eliminar", "Cancelar", "Escriba aquí");
            if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
            {
                return false;
            }

            //var approved = await authServ.AuthRoot(serverURL, pwd);
            //if (!approved)
            //{
            //    await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            //    return false;
            //}

            return true;
        }

        return await Shell.Current.DisplayAlert("Eliminar", message, "Eliminar", "Cancelar");
    }

    bool ConceptForDeletedInvoiceIsEmpty(ConceptForDeletedSaleRecord obj)
    {
        return obj is null || (obj.Id < 1 && (string.IsNullOrEmpty(obj.Concept) || string.IsNullOrWhiteSpace(obj.Concept)));
    }
    #endregion
}
