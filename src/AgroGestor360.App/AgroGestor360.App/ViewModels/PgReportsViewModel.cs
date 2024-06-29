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
        IsSelectedSale = true;
    }

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
    ObservableCollection<DTO6> sellers;

    [ObservableProperty]
    DTO6? selectedSeller;

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(IsSelectedSale))
        {
            if (IsSelectedSale)
            {
                IsEnabledToolBar = true;
            }
        }
    }

    public async void Initialize()
    {
        if (await sellersServ.ExistAsync(serverURL))
        {
            Sellers = new(await sellersServ.GetAllAsync(serverURL));
        }
    }
    #region EXTRA

    #endregion
}
