using AgroGestor360.App.Views.Settings;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class CvCustomersViewModel : ObservableRecipient
{
    readonly ICustomersService customersServ;
    readonly ILineCreditsService lineCreditsServ;
    readonly ITimeLimitsCreditsService timeLimitsCreditsServ;
    readonly IDiscountsCustomersService discountsCustomersServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvCustomersViewModel(ICustomersService customersService, ILineCreditsService lineCreditsService, ITimeLimitsCreditsService timeLimitsCreditsService, IDiscountsCustomersService discountsCustomersService, IAuthService authService)
    {
        customersServ = customersService;
        lineCreditsServ = lineCreditsService;
        timeLimitsCreditsServ = timeLimitsCreditsService;
        discountsCustomersServ = discountsCustomersService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    ObservableCollection<DTO5_1>? customers;

    [ObservableProperty]
    DTO5_1? selectedCustomer;

    [RelayCommand]
    async Task ShowSetCredit()
    {
        IsActive = true;
        var credits = await lineCreditsServ.GetAllAsync(serverURL);
        var timeLimits = await timeLimitsCreditsServ.GetAllAsync(serverURL);
        var defaulttimelimits = await timeLimitsCreditsServ.GetDefaultAsync(serverURL);
        Dictionary<string, object> sendObjects = new()
        {
            { "canceltoken", nameof(PgAddEditCustomer) },
            { "currentcustomer", SelectedCustomer! },
            {"credits", credits.ToArray() },
            {"timelimits", timeLimits.ToArray() },
            {"defaulttimelimits", defaulttimelimits! }
        };
        await Shell.Current.GoToAsync(nameof(PgSetCreditForCustomer), true, sendObjects);
    }

    [RelayCommand]
    async Task ClearCredit()
    {
        DTO5_5 dTO = new() { CustomerId = SelectedCustomer!.CustomerId, Credit = null };
        var result = await customersServ.UpdateCreditAsync(serverURL, dTO);
        if (result)
        {
            int idx = Customers!.IndexOf(SelectedCustomer!);
            var currentCustomer = Customers![idx];
            currentCustomer.Credit = null;
            Customers[idx] = currentCustomer;
        }
    }

    [RelayCommand]
    async Task SetDiscount()
    {
        if (EnableGetByDiscount)
        {
            StringBuilder sb = new();
            sb.AppendLine($"¿Va a modificar el descuento al cliente: {SelectedCustomer!.CustomerName}?");
            sb.AppendLine("");
            sb.AppendLine("Inserte la contraseña:");
            var pwd = await Shell.Current.DisplayPromptAsync("Modificar descuento", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
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
        }

        var options = await discountsCustomersServ.GetAllAsync(serverURL);
        if (options is not null)
        {
            var selectedOpt = await Shell.Current.DisplayActionSheet("Seleccione un descuento", "Cancelar", null, options.Select(x => $"{x.Name} - {x.Discount}%").ToArray());
            if (!string.IsNullOrEmpty(selectedOpt) && selectedOpt != "Cancelar")
            {
                var seccion = selectedOpt.Split('-');
                var discount = options.FirstOrDefault(x => x.Name == seccion[0].Trim());

                var result = await customersServ.UpdateDiscountAsync(serverURL, new() { CustomerId = SelectedCustomer!.CustomerId, DiscountId = discount!.Id });
                if (result)
                {
                    if (EnableGetByDiscount)
                    {
                        int idx = Customers!.IndexOf(SelectedCustomer!);
                        Customers[idx] = new() { CustomerId = SelectedCustomer!.CustomerId, CustomerName = SelectedCustomer!.CustomerName, Discount = discount };
                    }
                    else
                    {
                        Customers!.Remove(SelectedCustomer);
                    }
                }
            }
        }
        SelectedCustomer = null;
    }

    [RelayCommand]
    async Task UnSetDiscount()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el descuento al cliente: {SelectedCustomer!.CustomerName}?");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar descuento", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
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

        var result = await customersServ.UpdateDiscountAsync(serverURL, new() { CustomerId = SelectedCustomer!.CustomerId, DiscountId = 0 });
        if (result)
        {
            Customers!.Remove(SelectedCustomer);
        }
        SelectedCustomer = null;
    }


    [RelayCommand]
    async Task ShowAddCustomer()
    {
        IsActive = true;
        DiscountForCustomer[] discounts = (await discountsCustomersServ.GetAllAsync(serverURL)).ToArray();
        LineCreditItem[] credits = (await lineCreditsServ.GetAllAsync(serverURL)).ToArray();
        var sendObject = new Dictionary<string, object>() {
            { "Discounts", discounts },
            { "credits", credits } };
        await Shell.Current.GoToAsync(nameof(PgAddEditCustomer), true, sendObject);
    }

    [RelayCommand]
    async Task ShowEditCustomer()
    {
        IsActive = true;
        DiscountForCustomer[] discounts = (await discountsCustomersServ.GetAllAsync(serverURL)).ToArray();
        var currentCustomer = await customersServ.GetByIdAsync(serverURL, SelectedCustomer!.CustomerId!);
        var sendObject = new Dictionary<string, object>() {
            { "Discounts", discounts },
            { "CurrentCustomer", currentCustomer! }
        };
        await Shell.Current.GoToAsync(nameof(PgAddEditCustomer), true, sendObject);
    }

    [RelayCommand]
    async Task ShowDeleteCustomer()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el cliente: {SelectedCustomer!.CustomerName}?");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar cliente", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
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

        var result = await customersServ.DeleteAsync(serverURL, SelectedCustomer!.CustomerId!);
        if (result)
        {
            Customers!.Remove(SelectedCustomer);
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, DTO5_2, string>(this, "newCustomer", async (r, m) =>
        {
            string result = await customersServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Customers ??= [];
                r.Customers.Insert(0, new() { CustomerId = result, CustomerName = m.CustomerFullName, Discount = m.Discount });
            }
            r.SelectedCustomer = null;
            IsActive = false;
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, DTO5_3, string>(this, "editCustomer", async (r, m) =>
        {
            bool result = await customersServ.UpdateAsync(serverURL, m);
            if (result)
            {
                int idx = r.Customers!.IndexOf(SelectedCustomer!);
                r.Customers[idx] = new DTO5_1() { CustomerId = m.CustomerId, CustomerName = m.CustomerFullName, Discount = m.Discount };
            }

            r.SelectedCustomer = null;
            IsActive = false;
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, LineCredit, string>(this, "setcreditforcustomer", async (r, m) =>
        {
            DTO5_5 dTO = new() { CustomerId = SelectedCustomer!.CustomerId, Credit = m };
            bool result = await customersServ.UpdateCreditAsync(serverURL, dTO);
            if (result)
            {
                int idx = Customers!.IndexOf(SelectedCustomer!);
                var currentCustomer = Customers![idx];
                currentCustomer.Credit = m;
                Customers[idx] = currentCustomer;
            }

            r.SelectedCustomer = null;
            IsActive = false;
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, string, string>(this, nameof(PgAddEditCustomer), (r, m) =>
        {
            if (m == "cancel")
            {
                r.SelectedCustomer = null;
                IsActive = false;
            }
        });
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
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
        _ = await customersServ.ExistAsync(serverURL);
    }
    #endregion
}
