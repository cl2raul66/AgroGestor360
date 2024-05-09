using AgroGestor360.App.Views;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360.App.ViewModels;

public partial class PgHomeViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IApiService apiServ;

    public PgHomeViewModel(IApiService apiService)
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        apiServ = apiService;
        CheckUrl();
    }

    [ObservableProperty]
    bool serverConnected;

    [RelayCommand]
    async Task GoToSettings() => await Shell.Current.GoToAsync(nameof(PgSettings), true);

    [RelayCommand]
    async Task GoToSales() => await Shell.Current.GoToAsync(nameof(PgSales), true);

    #region EXTRA
    async void CheckUrl()
    {
        ServerConnected = await apiServ.CheckUrl(serverURL);
    }
    #endregion
}
