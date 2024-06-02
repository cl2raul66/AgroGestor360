using AgroGestor360.App.Views;
using AgroGestor360.Client.Services;
using AgroGestor360.Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;

namespace AgroGestor360.App.ViewModels;

public partial class PgHomeViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IApiService apiServ;

    public PgHomeViewModel(IApiService apiService)
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        apiServ = apiService;
        apiServ.OnReceiveStatusMessage += ApiServ_OnReceiveStatusMessage;           
        AppInfo = $"{Assembly.GetExecutingAssembly().GetName().Name} V.{VersionTracking.Default.CurrentVersion}";
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

    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }

    #region EXTRA
    public async void Initialize()
    {
        ServerConnected = await apiServ.CheckUrl(serverURL);
        HaveConnection = await apiServ.ConnectToServerHub(serverURL);
    }    
    #endregion
}
