using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Services;
using AgroGestor360Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AgroGestor360App.ViewModels;

public partial class CvConnectionViewModel : ObservableRecipient
{
    readonly IApiService apiServ;

    public CvConnectionViewModel(IApiService apiService)
    {
        IsActive = true;
        ServerURL = Preferences.Default.Get<string?>("serverurl", null);
        apiServ = apiService;
        if (!string.IsNullOrEmpty(ServerURL))
        {
            Task.Run(async () =>
            {
                HaveConnection = await apiServ.ConnectToServerHub(ServerURL);
            });
        }
        apiServ.OnReceiveStatusMessage += ApiServ_OnReceiveStatusMessage;
    }

    [ObservableProperty]
    bool haveConnection;

    [ObservableProperty]
    string? serverURL;

    [RelayCommand]
    async Task ShowSetURL()
    {
        await Shell.Current.GoToAsync(nameof(PgSetURL), true);
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvConnectionViewModel, string, string>(this, nameof(PgSetURL), (r, m) =>
        {
            r.ServerURL = m;
        });
    }

    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }
}
