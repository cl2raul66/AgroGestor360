using AgroGestor360.App.Views.Settings.Connection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AgroGestor360.App.ViewModels;

public partial class CvConnectionViewModel : ObservableRecipient
{
    public CvConnectionViewModel()
    {
        IsActive = true;
        ServerURL = Preferences.Default.Get<string?>("serverurl", null);
    }

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
        WeakReferenceMessenger.Default.Register<CvConnectionViewModel,string, string>(this, nameof(PgSetURL), (r, m) =>
        {
            r.ServerURL = m;
        });
    }
}
