using AgroGestor360.App.Views.Settings;
using AgroGestor360.Client.Models;
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
    readonly ISellerService sellerServ;
    readonly string serverURL;

    public CvSellersViewModel(ISellerService sellerService)
    {
        sellerServ = sellerService;
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
        sb.AppendLine("Usted esta seguro de eliminar al siguiente vendedor:");
        sb.AppendLine($"FECHA DE INGRESO: {0}");
        sb.AppendLine($"NOMBRE: {0}");
        sb.AppendLine($"NIT: {0}");
        sb.AppendLine($"DPI: {0}");
        sb.AppendLine($"DIRECCION: {0}");
        sb.AppendLine($"TELEFONO: {0}");
        sb.AppendLine($"CORREO ELECTRONICO: {0}");

        bool resul = await Shell.Current.DisplayAlert("Eliminar cliente", sb.ToString().TrimEnd(), "Eliminar", "Cancelar");
        if (resul)
        {
            return;
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvSellersViewModel, vCard, string>(this, "newSeller", async (r, m) =>
        {
            bool result = await sellerServ.InsertAsync(serverURL, m);
            if (result)
            {
                r.Sellers ??= [];
                r.Sellers.Insert(0, m);
            }

            r.SelectedSeller = null;
        });

        WeakReferenceMessenger.Default.Register<CvSellersViewModel, vCard, string>(this, "editSeller", async (r, m) =>
        {
            bool result = await sellerServ.UpdateAsync(serverURL, m);
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
        bool exist = await sellerServ.ExistAsync(serverURL);
        if (exist)
        {
            var getsellers = await sellerServ.GetAllAsync(serverURL);
            Sellers = new(getsellers!);
        }
    }
    #endregion
}
