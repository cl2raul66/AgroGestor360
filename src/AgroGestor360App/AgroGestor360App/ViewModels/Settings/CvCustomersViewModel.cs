using AgroGestor360App.Views.Dialogs;
using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AgroGestor360App.ViewModels;

public partial class CvCustomersViewModel : ObservableRecipient
{
    readonly ICustomersService customersServ;
    readonly ILineCreditsService lineCreditsServ;
    readonly ITimeLimitsCreditsService timeLimitsCreditsServ;
    readonly IDiscountsCustomersService discountsCustomersServ;
    readonly string serverURL;

    public CvCustomersViewModel(ICustomersService customersService, ILineCreditsService lineCreditsService, ITimeLimitsCreditsService timeLimitsCreditsService, IDiscountsCustomersService discountsCustomersService)
    {
        customersServ = customersService;
        lineCreditsServ = lineCreditsService;
        timeLimitsCreditsServ = timeLimitsCreditsService;
        discountsCustomersServ = discountsCustomersService;
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
            { "credits", credits.ToArray() },
            { "timelimits", timeLimits.ToArray() },
            { "defaulttimelimits", defaulttimelimits! }
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
        IsActive = true;
        var options = await discountsCustomersServ.GetAllAsync(serverURL);
        Dictionary<string, object> sendData = new()
        {
            {"Options", options.ToArray()},
            {"SendToken", "71594d2a-3954-4591-807b-3f0328495550"}
        };
        await Shell.Current.GoToAsync(nameof(PgSelectDiscountsOptionsDialog), true, sendData);
    }

    [RelayCommand]
    async Task UnSetDiscount()
    {
        var result = await customersServ.UpdateDiscountAsync(serverURL, new() { CustomerId = SelectedCustomer!.CustomerId, DiscountId = 0 });
        if (result)
        {
            int idx = Customers!.IndexOf(SelectedCustomer!);
            var modifiedCustomer = await customersServ.GetByIdAsync(serverURL, SelectedCustomer!.CustomerId!);
            if (modifiedCustomer is not null)
            {
                Customers[idx] = modifiedCustomer;
            }
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
            IsActive = false;
            string result = await customersServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Customers ??= [];
                r.Customers.Insert(0, new() { CustomerId = result, CustomerName = m.CustomerFullName, Discount = m.Discount });
            }
            r.SelectedCustomer = null;
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, DTO5_3, string>(this, "editCustomer", async (r, m) =>
        {
            IsActive = false;
            bool result = await customersServ.UpdateAsync(serverURL, m);
            if (result)
            {
                int idx = r.Customers!.IndexOf(SelectedCustomer!);
                r.Customers[idx] = new DTO5_1() { CustomerId = m.CustomerId, CustomerName = m.CustomerFullName, Discount = m.Discount };
            }

            r.SelectedCustomer = null;
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, LineCredit, string>(this, "setcreditforcustomer", async (r, m) =>
        {
            IsActive = false;
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
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, string, string>(this, "16030c9d-7d3f-11eb-9631-0242ac130002", (r, m) =>
        {
                IsActive = false;
            if (m == "cancel")
            {
                r.SelectedCustomer = null;
            }
        });

        WeakReferenceMessenger.Default.Register<CvCustomersViewModel, DiscountForCustomer, string>(this, "71594d2a-3954-4591-807b-3f0328495550", async (r, m) =>
        {
            IsActive = false;
            if (m is not null)
            {
                var result = await customersServ.UpdateDiscountAsync(serverURL, new() { CustomerId = SelectedCustomer!.CustomerId, DiscountId = m.Id });
                if (result)
                {
                    int idx = Customers!.IndexOf(SelectedCustomer!);
                    var modifiedCustomer = await customersServ.GetByIdAsync(serverURL, SelectedCustomer!.CustomerId!);
                    if (modifiedCustomer is not null)
                    {
                        Customers[idx] = modifiedCustomer;
                    }
                }
            }
            r.SelectedCustomer = null;
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
        bool exist = await customersServ.ExistAsync(serverURL);
        if (exist)
        {
            Customers = [.. await customersServ.GetAllAsync(serverURL)];
        }
    }
    #endregion
}
