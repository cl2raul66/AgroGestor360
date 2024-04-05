using AgroGestor360.App.Views.Settings;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using vCardLib.Models;

namespace AgroGestor360.App.ViewModels;

public partial class CvSellersViewModel : ObservableRecipient
{
    readonly ISellersService sellersServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvSellersViewModel(ISellersService sellersService, IAuthService authService)
    {
        sellersServ = sellersService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        IsActive = true;
    }

    [ObservableProperty]
    ObservableCollection<vCard>? sellers;

    [ObservableProperty]
    vCard? selectedSeller;

    [RelayCommand]
    async Task ShowAddSeller()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditSeller), true);
    }

    [RelayCommand]
    async Task ShowEditSeller()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditSeller), true, new Dictionary<string, object>() { { "CurrentSeller", SelectedSeller! } });
    }

    [RelayCommand]
    async Task ShowDeleteSeller()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el vendedor: {SelectedSeller!.FormattedName}?");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar vendedor", sb.ToString(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedSeller = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedSeller = null;
            return;
        }

        var result = await sellersServ.DeleteAsync(serverURL, SelectedSeller!.Uid!);
        if (result)
        {
            Sellers!.Remove(SelectedSeller);
            SelectedSeller = null;
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvSellersViewModel, vCard, string>(this, "newSeller", async (r, m) =>
        {
            bool result = await sellersServ.InsertAsync(serverURL, m);
            if (result)
            {
                r.Sellers ??= [];
                r.Sellers.Insert(0, m);
            }

            r.SelectedSeller = null;
        });

        WeakReferenceMessenger.Default.Register<CvSellersViewModel, vCard, string>(this, "editSeller", async (r, m) =>
        {
            bool result = await sellersServ.UpdateAsync(serverURL, m);
            if (result)
            {
                int idx = r.Sellers!.IndexOf(SelectedSeller!);
                r.Sellers[idx] = m;
            }

            r.SelectedSeller = null;
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        await GetSellers();
    }

    private async Task GetSellers()
    {
        bool exist = await sellersServ.ExistAsync(serverURL);
        if (exist)
        {
            var getsellers = await sellersServ.GetAllAsync(serverURL);
            Sellers = new(getsellers!);
        }
    }
    #endregion
}
