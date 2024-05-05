using AgroGestor360.App.Views.Settings.Customers;
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
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvCustomersViewModel(ICustomersService customersService, IAuthService authService)
    {
        customersServ = customersService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
    }

    [ObservableProperty]
    ObservableCollection<DTO5_1>? customers;

    [ObservableProperty]
    DTO5_1? selectedCustomer;

    [ObservableProperty]
    bool enableGetByDiscount = true;

    [RelayCommand]
    void ChangeEnableGetByDiscount()
    {
        EnableGetByDiscount = !EnableGetByDiscount;
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

        var options = await customersServ.GetAllDiscountAsync(serverURL);
        if (options is not null)
        {
            var selectedOpt = await Shell.Current.DisplayActionSheet("Seleccione un descuento", "Cancelar", null, options.Select(x => $"{x.Name} - {x.Value}%").ToArray());
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

                        if (Customers.Count == 0)
                        {
                            ChangeEnableGetByDiscount();
                        }
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

            if (Customers.Count == 0)
            {
                ChangeEnableGetByDiscount();
            }
        }
        SelectedCustomer = null;
    }


    [RelayCommand]
    async Task ShowAddCustomer()
    {
        IsActive = true;
        CustomerDiscountClass[] discounts = (await customersServ.GetAllDiscountAsync(serverURL)).ToArray();
        var sendObject = new Dictionary<string, object>() { { "Discounts", discounts } };
        await Shell.Current.GoToAsync(nameof(PgAddEditCustomer), true, sendObject);
    }

    [RelayCommand]
    async Task ShowEditCustomer()
    {
        IsActive = true;
        CustomerDiscountClass[] discounts = (await customersServ.GetAllDiscountAsync(serverURL)).ToArray();
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
            if (!Customers.Any())
            {
                ChangeEnableGetByDiscount();
            }
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
                if (EnableGetByDiscount)
                {
                    ChangeEnableGetByDiscount();
                }
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

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, string, string>(this, nameof(PgAddEditCustomer), (r, m) =>
        {
            if (m == "cancel")
            {
                r.SelectedCustomer = null;
                IsActive = false;
            }
        });
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(EnableGetByDiscount))
        {
            Customers ??= [];
            if (EnableGetByDiscount)
            {
                Customers = new(await customersServ.GetAllWithDiscountAsync(serverURL));
            }
            else
            {
                Customers = new(await customersServ.GetAllWithoutDiscountAsync(serverURL));
                if (Customers?.Count == 0)
                {
                    EnableGetByDiscount = true;
                }
            }
            SelectedCustomer = null;
        }
    }

    #region EXTRA
    public async void Initialize()
    {
        await GetCustomers();
    }

    async Task GetCustomers()
    {
        bool exist = await customersServ.ExistAsync(serverURL);
        if (exist)
        {
            EnableGetByDiscount = false;
        }
    }
    #endregion
}
