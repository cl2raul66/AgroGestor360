using AgroGestor360.App.Views.Settings;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;

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
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    ObservableCollection<DTO6>? sellers;

    [ObservableProperty]
    DTO6? selectedSeller;

    [RelayCommand]
    async Task ShowAddSeller()
    {
        IsActive = true;
        await Shell.Current.GoToAsync(nameof(PgAddEditSeller), true);
    }

    [RelayCommand]
    async Task ShowEditSeller()
    {
        IsActive = true;

        StringBuilder sb = new();
        sb.AppendLine($"Usted va a editar el vendedor: {SelectedSeller!.FullName}");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("editar vendedor", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
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

        var currentSeller = await sellersServ.GetByIdAsync(serverURL, SelectedSeller!.Id!);
        await Shell.Current.GoToAsync(nameof(PgAddEditSeller), true, new Dictionary<string, object>() { { "CurrentSeller", currentSeller! } });
    }

    [RelayCommand]
    async Task ShowDeleteSeller()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el vendedor: {SelectedSeller!.FullName}?");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar vendedor", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
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

        var result = await sellersServ.DeleteAsync(serverURL, SelectedSeller!.Id!);
        if (result)
        {
            Sellers!.Remove(SelectedSeller);
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvSellersViewModel, DTO6_1, string>(this, "newSeller", async (r, m) =>
        {
            string result = await sellersServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Sellers ??= [];
                r.Sellers.Insert(0, new() { FullName = m.FullName, Id = result });
            }

            IsActive = false;
            r.SelectedSeller = null;
        });

        WeakReferenceMessenger.Default.Register<CvSellersViewModel, DTO6_2, string>(this, "editSeller", async (r, m) =>
        {
            bool result = await sellersServ.UpdateAsync(serverURL, m);
            if (result)
            {
                int idx = r.Sellers!.IndexOf(r.SelectedSeller!);
                r.Sellers[idx] = new() { FullName = m.FullName, Id = m.Id };
            }

            IsActive = false;
            r.SelectedSeller = null;
        });

        WeakReferenceMessenger.Default.Register<CvSellersViewModel, string, string>(this, nameof(PgAddEditSeller), (r, m) =>
        {
            if (m == "cancel")
            {
                IsActive = false;
                r.SelectedSeller = null;
            }
        });
        //todo: implementar mensaje de cancelacion
    }

    #region EXTRA
    public async void Initialize()
    {
        IsBusy = true;
        await GetSellers();
        IsBusy = false;
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
