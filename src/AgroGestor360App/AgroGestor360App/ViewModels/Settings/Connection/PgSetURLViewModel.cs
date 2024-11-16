using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360App.ViewModels;

public partial class PgSetURLViewModel : ObservableValidator
{
    readonly IApiService apiServ;

    public PgSetURLViewModel(IApiService apiService)
    {
        apiServ = apiService;
    }

    [ObservableProperty]
    [Required]
    [Url]
    string? serverURL;

    [ObservableProperty]
    bool? connectedAPI;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Test()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            return;
        }

        ConnectedAPI = await apiServ.CheckUrl(ServerURL!);
    }

    [RelayCommand]
    async Task Save()
    {
        Preferences.Default.Set("serverurl", ServerURL);
        _ = WeakReferenceMessenger.Default.Send(ServerURL!, nameof(PgSetURL));
        await Cancel();
    }
}
