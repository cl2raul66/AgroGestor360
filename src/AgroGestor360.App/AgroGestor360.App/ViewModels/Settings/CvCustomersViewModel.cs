using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Settings.Customers;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using vCardLib.Models;

namespace AgroGestor360.App.ViewModels;

public partial  class CvCustomersViewModel : ObservableRecipient
{
    readonly ICustomersService customersServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvCustomersViewModel(ICustomersService customersService, IAuthService authService)
    {
        customersServ = customersService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        IsActive = true;
    }

    [ObservableProperty]
    ObservableCollection<vCard>? customers;

    [ObservableProperty]
    vCard? selectedCustomer;

    [RelayCommand]
    async Task ShowAddCustomer()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditCustomer), true);
    }

    [RelayCommand]
    async Task ShowEditCustomer()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditCustomer), true, new Dictionary<string, object>() { { "CurrentCustomer", SelectedCustomer! } });
    }

    [RelayCommand]
    async Task ShowDeleteCustomer()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el cliente: {SelectedCustomer!.FormattedName}?");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar cliente", sb.ToString(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedCustomer = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedCustomer = null;
            return;
        }

        var result = await customersServ.DeleteAsync(serverURL, SelectedCustomer!.Uid!);
        if (result)
        {
            Customers!.Remove(SelectedCustomer);
            SelectedCustomer = null;
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, vCard, string>(this, "newCustomer", async (r, m) =>
        {
            bool result = await customersServ.InsertAsync(serverURL, m);
            if (result)
            {
                r.Customers ??= [];
                r.Customers.Insert(0, m);
            }

            r.SelectedCustomer = null;
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, vCard, string>(this, "editCustomer", async (r, m) =>
        {
            bool result = await customersServ.UpdateAsync(serverURL, m);
            if (result)
            {
                int idx = r.Customers!.IndexOf(SelectedCustomer!);
                r.Customers[idx] = m;
            }

            r.SelectedCustomer = null;
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        await GetSellers();
    }

    private async Task GetSellers()
    {
        bool exist = await customersServ.ExistAsync(serverURL);
        if (exist)
        {
            var getsellers = await customersServ.GetAllAsync(serverURL);
            Customers = new(getsellers!);
        }
    }
    #endregion
}
