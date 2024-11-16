// Ignore Spelling: api

using AgroGestor360App.Models;
using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using AgroGestor360Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AgroGestor360App.ViewModels;

[QueryProperty(nameof(ServerConnected), "serverConnected")]
[QueryProperty(nameof(HaveConnection), "haveConnection")]
public partial class PgReportsViewModel : ObservableObject
{
    readonly string serverURL;
    readonly IApiService apiServ;
    readonly ICustomersService customersServ;
    readonly ISellersService sellersServ;
    readonly IReportsService reportsServ;

    public PgReportsViewModel(IApiService apiService, ICustomersService customersService, ISellersService sellersService, IReportsService reportsService)
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        apiServ = apiService;
        apiServ.OnReceiveStatusMessage += ApiServ_OnReceiveStatusMessage;
        customersServ = customersService;
        sellersServ = sellersService;
        reportsServ = reportsService;

        AppInfo = $"Versión: {VersionTracking.Default.CurrentVersion}";
        ReportsMenu = [
            new MenuItemReport() { Title = "Ventas", Description = "Reporte de ventas." },
            new MenuItemReport() { Title = "Arqueo de caja", Description = "Reporte de arqueos de caja." }
        ];
    }

    [ObservableProperty]
    bool isVisibleMenu;

    [ObservableProperty]
    bool haveConnection;

    [ObservableProperty]
    string? appInfo;

    [ObservableProperty]
    bool serverConnected;

    [ObservableProperty]
    bool isSelectedSale;

    [ObservableProperty]
    bool isEnabledToolBar;

    [ObservableProperty]
    bool isFoundElement;

    [ObservableProperty]
    bool isEnableSearch;

    [ObservableProperty]
    List<MenuItemReport> reportsMenu;

    [ObservableProperty]
    MenuItemReport? selectedMenu;

    [ObservableProperty]
    ObservableCollection<DTO5_1>? customers;

    [ObservableProperty]
    ObservableCollection<DTO6>? sellers;

    #region SALE
    [ObservableProperty]
    bool isFiltredByCustomer;

    [ObservableProperty]
    DTO5_1? selectedCustomer;

    [ObservableProperty]
    bool isFiltredBySeller;

    [ObservableProperty]
    DTO6? selectedSeller;

    [ObservableProperty]
    bool isFiltredByDates;

    [ObservableProperty]
    DateTime? beginDate;

    [ObservableProperty]
    DateTime endDate = DateTime.Now;

    [ObservableProperty]
    bool isFiltredByStates;

    [ObservableProperty]
    bool isStatePaid;

    [ObservableProperty]
    bool isStatePending;

    [ObservableProperty]
    bool isStateCancelled;

    [ObservableProperty]
    ObservableCollection<SaleReport.SaleTable>? saleTableItems;

    [ObservableProperty]
    double totalPaid;

    [ObservableProperty]
    double totalToPay;

    SaleReport? saleReport;
    #endregion

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    void ShowMenu()
    {
        IsVisibleMenu = !IsVisibleMenu;
    }

    [RelayCommand]
    async Task Search()
    {
        if (IsSelectedSale)
        {
            string reportState = string.Empty;

            if (!IsFiltredByStates)
            {
                reportState = "Todas";
            }
            else if (IsStatePaid)
            {
                reportState = "Pagadas";
            }
            else if (IsStatePending)
            {
                reportState = "Pendientes";
            }
            else if (IsStateCancelled)
            {
                reportState = "Canceladas";
            }

            SaleReportParameters parameters = new(
                reportState,
                "NONE",
                BeginDate,
                EndDate,
                SelectedCustomer?.CustomerId,
                SelectedSeller?.Id);

            saleReport = await reportsServ.GetSaleReportReportAsync(serverURL, parameters);

            SaleTableItems = saleReport is null ? null : new(saleReport.SaleItems!);
            TotalPaid = saleReport?.SaleItems?.Sum(x => x.TotalPaid) ?? 0;
            TotalToPay = saleReport?.SaleItems?.Sum(x => x.TotalToPay) ?? 0;
        }
    }

    [RelayCommand]
    async Task ShareAsPdf()
    {
        IsFoundElement = false;
        string filePath = Path.Combine(FileSystem.CacheDirectory, "REPORTE DE VENTAS.pdf");
        string result = await reportsServ.GeneratePDFSaleReportAsync(serverURL, saleReport!, filePath);
        if (!string.IsNullOrEmpty(result))
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Compartir cotización seleccionada",
                File = new ShareFile(result)
            });
        }
        IsFoundElement = true;
    }

    [RelayCommand]
    async Task ShowAsPdf()
    {
        IsFoundElement = false;
        string filePath = Path.Combine(FileSystem.CacheDirectory, "REPORTE DE VENTAS.pdf");
        string result = await reportsServ.GeneratePDFSaleReportAsync(serverURL, saleReport!, filePath);
        if (!string.IsNullOrEmpty(result))
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(result)
            });
        }
        IsFoundElement = true;
    }

    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }

    partial void OnSaleTableItemsChanged(ObservableCollection<SaleReport.SaleTable>? value)
    {
        IsFoundElement = value is not null && value.Count > 0;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(HaveConnection))
        {
            IsEnabledToolBar = HaveConnection;
            IsVisibleMenu = HaveConnection;
        }

        if (e.PropertyName == nameof(IsSelectedSale))
        {
            if (IsSelectedSale)
            {
                IsEnableSearch = true;
            }
        }

        if (e.PropertyName == nameof(SelectedMenu))
        {
            switch (SelectedMenu?.Title)
            {
                case "Ventas":
                    IsSelectedSale = true;
                    break;
                case "Cierre de caja":
                    IsSelectedSale = false;
                    break;
                case "Conciliación de caja":
                    IsSelectedSale = false;
                    break;
                default:
                    break;
            }

            if (SelectedMenu is not null)
            {
                IsVisibleMenu = false;
                SelectedMenu = null;
            }
        }

        if (e.PropertyName == nameof(IsFiltredByCustomer))
        {
            SelectedCustomer = null;
        }

        if (e.PropertyName == nameof(IsFiltredBySeller))
        {
            SelectedSeller = null;
        }

        if (e.PropertyName == nameof(IsFiltredByDates))
        {
            BeginDate = null;
        }

        if (e.PropertyName == nameof(IsFiltredByStates))
        {
            IsStatePaid = IsFiltredByStates;
        }
    }

    public async void Initialize()
    {
        if (await customersServ.ExistAsync(serverURL))
        {
            Customers = new(await customersServ.GetAllAsync(serverURL));
        }
        if (await sellersServ.ExistAsync(serverURL))
        {
            Sellers = new(await sellersServ.GetAllAsync(serverURL));
        }

        await Task.Run(() =>
        {
            foreach (var f in Directory.GetFiles(FileSystem.CacheDirectory, "*.pdf"))
            {
                File.Delete(f);
            }
        });
    }

    #region EXTRA

    #endregion
}
