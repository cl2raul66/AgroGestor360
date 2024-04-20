using AgroGestor360.App.Views.Settings.Sales;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AgroGestor360.App.ViewModels;

public partial class CvSalesViewModel : ObservableRecipient
{
    readonly ITypesDiscountsCustomersService typesDiscountsCustomersServ;
    readonly string serverURL;

    public CvSalesViewModel(ITypesDiscountsCustomersService typesDiscountsCustomersService)
    {
        typesDiscountsCustomersServ = typesDiscountsCustomersService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        EnableDiscountDefault = Preferences.Default.Get("EnableDiscountDefault", false);
        IsActive = true;
    }

    [ObservableProperty]
    bool? enableDiscountDefault;

    [ObservableProperty]
    ObservableCollection<CustomerDiscountClass>? discount;

    [ObservableProperty]
    CustomerDiscountClass? selectedDiscount;

    [RelayCommand]
    async Task ShowAddDiscount() => await Shell.Current.GoToAsync(nameof(PgAddCustomerDiscountType), true);

    [RelayCommand]
    async Task DeleteDiscount()
    {
        bool result = await typesDiscountsCustomersServ.DeleteAsync(serverURL, SelectedDiscount!.Id);
        if (result)
        {
            Discount!.Remove(SelectedDiscount);
        }
    }

    [RelayCommand]
    void SetEnableDiscountDefault()
    {
        EnableDiscountDefault = !EnableDiscountDefault;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvSalesViewModel, CustomerDiscountClass, string>(this, "addcustomerdiscounttype", async (r, m) =>
        {
            int result = await typesDiscountsCustomersServ.InsertAsync(serverURL, m);
            if (result > 0)
            {
                r.Discount ??= [];
                m.Id = result;
                r.Discount.Insert(0, m);
            }
        });
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(EnableDiscountDefault))
        {
            Discount ??= [];
            if (EnableDiscountDefault!.Value)
            {
                Discount = new(await typesDiscountsCustomersServ.GetAllDefaultAsync(serverURL));
            }
            else
            {
                Discount = new(await typesDiscountsCustomersServ.GetAllAsync(serverURL));
            }
            Preferences.Set("EnableDiscountDefault", EnableDiscountDefault!.Value);
            SelectedDiscount = null;
        }
    }

    #region EXTRA
    public async void Initialize()
    {
        //await Task.WhenAll(GetArticles(), GetProducts());
        await GetDiscount();
    }

    async Task GetDiscount()
    {
        await Task.CompletedTask;
        //Discount ??= [];
        //if (EnableDiscountDefault)
        //{
        //    Discount = new(await typesDiscountsCustomersServ.GetAllAsync(serverURL));
        //}
        //else
        //{
        //    Discount = new(await typesDiscountsCustomersServ.GetAllDefaultAsync(serverURL));
        //}
        //EnableDiscountDefault = !EnableDiscountDefault;
        //Preferences.Set("EnableDiscountDefault", !EnableDiscountDefault);
    }
    #endregion
}
