using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;

namespace AgroGestor360App.ViewModels;

public partial class CvDiscountsViewModel : ObservableRecipient
{
    readonly IDiscountsCustomersService discountsCustomersServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvDiscountsViewModel(IDiscountsCustomersService discountsCustomersService, IAuthService authService)
    {
        discountsCustomersServ = discountsCustomersService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    ObservableCollection<DiscountForCustomer>? discountForCustomers;

    [ObservableProperty]
    DiscountForCustomer? selectedDiscount;

    [RelayCommand]
    async Task ShowNewDiscountForCustomer()
    {
        IsActive = true;
        await Shell.Current.GoToAsync(nameof(PgAddDiscountForCustomer), true);
    }

    [RelayCommand]
    async Task ShowDeletedDiscountForCustomer()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Nombre: {SelectedDiscount!.Name}");
        sb.AppendLine($"Descuento: {SelectedDiscount!.Discount}%");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar descuento", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedDiscount = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedDiscount = null;
            return;
        }

        bool result = await discountsCustomersServ.DeleteAsync(serverURL, SelectedDiscount!.Id);
        if (result)
        {
            DiscountForCustomers!.Remove(SelectedDiscount!);
        }

        SelectedDiscount = null;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvDiscountsViewModel, DiscountForCustomer, string>(this, "adddiscountforcustomer", async (r, m) =>
        {
            int result = await discountsCustomersServ.InsertAsync(serverURL, m);
            if (result > 0)
            {
                DiscountForCustomers ??= [];
                m.Id = result;
                DiscountForCustomers.Insert(0, m);
            }

            SelectedDiscount = null;
            IsActive = false;
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        IsBusy = true;
        await GetCustomers();
        IsBusy = false;
    }

    async Task GetCustomers()
    {
        DiscountForCustomers = new(await discountsCustomersServ.GetAllAsync(serverURL));
    }
    #endregion
}
