using AgroGestor360.App.Models;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using AgroGestor360.Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(ServerConnected), "serverConnected")]
[QueryProperty(nameof(HaveConnection), "haveConnection")]
public partial class PgReportsViewModel : ObservableObject
{
    readonly string serverURL;
    readonly IApiService apiServ;
    readonly ICustomersService customersServ;
    readonly ISellersService sellersServ;

    public PgReportsViewModel(IApiService apiService, ICustomersService customersService, ISellersService sellersService)
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        apiServ = apiService;
        apiServ.OnReceiveStatusMessage += ApiServ_OnReceiveStatusMessage;
        customersServ = customersService;
        sellersServ = sellersService;

        AppInfo = $"Versión: {VersionTracking.Default.CurrentVersion}";
        ReportsMenu = [
            new MenuItemReport() { Title = "Ventas", Description = "Reporte de ventas." },
            new MenuItemReport() { Title = "Cierre de caja", Description = "Reporte de re conteo de las operaciones al final del día." },
            new MenuItemReport() { Title = "Conciliación de caja", Description = "Reporte de ventas." }
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
    bool isSelectedElement;

    [ObservableProperty]
    List<MenuItemReport> reportsMenu;

    [ObservableProperty]
    MenuItemReport? selectedMenu;

    [ObservableProperty]
    ObservableCollection<DTO5_1>? customers;

    [ObservableProperty]
    DTO5_1? selectedCustomer;

    [ObservableProperty]
    ObservableCollection<DTO6>? sellers;

    [ObservableProperty]
    DTO6? selectedSeller;

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    void ShowMenu()
    {
        IsVisibleMenu = !IsVisibleMenu;
    }

    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(HaveConnection))
        {
            IsEnabledToolBar = HaveConnection;
        }

        if (e.PropertyName == nameof(IsSelectedSale))
        {
            
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
            }
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
    }
    #region EXTRA

    #endregion
}
