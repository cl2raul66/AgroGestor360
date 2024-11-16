using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360App.ViewModels;
public partial class PgAddDiscountForCustomerViewModel : ObservableValidator
{
    [ObservableProperty]
    [Required]
    [MinLength(2)]
    string? name;

    [ObservableProperty]
    [Required]
    string? discount;

    [ObservableProperty]
    bool isVisisbleInfo;

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", nameof(CvDiscounts));
        await Shell.Current.GoToAsync("..", true);
    }

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors || !double.TryParse(Discount, out double theDiscount))
        {
            IsVisisbleInfo = true;
            await Task.Delay(4000);
            IsVisisbleInfo = false;

            return;
        }

        DiscountForCustomer discountForCustomer = new()
        {
            Name = Name!.Trim().ToUpper(),
            Discount = theDiscount
        };

        _ = WeakReferenceMessenger.Default.Send(discountForCustomer, "adddiscountforcustomer");

        await Shell.Current.GoToAsync("..", true);
    }
}
