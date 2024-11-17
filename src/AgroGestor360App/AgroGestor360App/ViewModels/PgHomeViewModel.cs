using AgroGestor360App.Views;
using AgroGestor360Client.Services;
using AgroGestor360Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360App.ViewModels;

public partial class PgHomeViewModel : ObservableRecipient
{
    readonly IProductsForSalesService productsForSalesServ;
    readonly ISellersService sellersServ;
    readonly ICustomersService customersServ;
    string? serverURL;
    readonly IApiService apiServ;

    public PgHomeViewModel(IApiService apiService, ISellersService sellersService, ICustomersService customersService, IProductsForSalesService productsForSalesService)
    {
        //serverURL = Preferences.Default.Get("serverurl", string.Empty);
        //if (string.IsNullOrEmpty(serverURL))
        //{
        //    var result = await apiServ.CheckUrl("http://localhost:5010");
        //    if (result)
        //    {
        //        serverURL = "http://localhost:5010";
        //        Preferences.Default.Set("serverurl", serverURL);
        //    }
        //}
        apiServ = apiService;
        apiServ.OnReceiveStatusMessage += ApiServ_OnReceiveStatusMessage;
        sellersServ = sellersService;
        customersServ = customersService;
        productsForSalesServ = productsForSalesService;
        AppInfo = $"Versión: {VersionTracking.Default.CurrentVersion}";
    }

    [ObservableProperty]
    bool haveConnection;

    [ObservableProperty]
    string? appInfo;

    [ObservableProperty]
    bool serverConnected;

    [RelayCommand]
    async Task GoToSettings() => await Shell.Current.GoToAsync(nameof(PgSettings), true);

    [RelayCommand]
    async Task GoToSales() => await Shell.Current.GoToAsync(nameof(PgSales), true);

    [RelayCommand]
    async Task GoToReports()
    {
        Dictionary<string, object> sendData = new()
        {
            { "serverConnected", ServerConnected },
            { "haveConnection", HaveConnection }
        };
        await Shell.Current.GoToAsync(nameof(PgReports), true, sendData);
    }

    [RelayCommand]
    async Task ShowAddQuote()
    {
        Dictionary<string, object> sendData = new()
        {
            { "ShowAddQuote", true }
        };
        await Shell.Current.GoToAsync(nameof(PgSales), true, sendData);
    }

    [RelayCommand]
    async Task ShowAddOrder()
    {
        Dictionary<string, object> sendData = new()
        {
            { "ShowAddOrder", true }
        };
        await Shell.Current.GoToAsync(nameof(PgSales), true, sendData);
    }

    [RelayCommand]
    async Task ShowAddSale()
    {
        Dictionary<string, object> sendData = new()
        {
            { "ShowAddSale", true }
        };
        await Shell.Current.GoToAsync(nameof(PgSales), true, sendData);
    }

    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }

    public async void Initialize()
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            if (string.IsNullOrEmpty(serverURL))
            {
                var result = await apiServ.CheckUrl("http://localhost:5010");
                if (result)
                {
                    serverURL = "http://localhost:5010";
                    Preferences.Default.Set("serverurl", serverURL);
                }

            }
        }

        ServerConnected = await apiServ.CheckUrl(serverURL);
        HaveConnection = await apiServ.ConnectToServerHub(serverURL);
    }

    #region EXTRA
    #endregion
}
