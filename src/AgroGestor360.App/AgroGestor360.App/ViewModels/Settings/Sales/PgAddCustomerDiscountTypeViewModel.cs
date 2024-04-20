using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

public partial class PgAddCustomerDiscountTypeViewModel : ObservableValidator
{
    [ObservableProperty]
    [Required]
    string? className;

    [ObservableProperty]
    [Required]
    string? value;

    [ObservableProperty]
    bool isVisibleInfo;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();
        if (HasErrors || !double.TryParse(Value, out double theValue))
        {
            IsVisibleInfo = true;
            await Task.Delay(4000);
            IsVisibleInfo = false;
            return;
        }

        _ = WeakReferenceMessenger.Default.Send(new CustomerDiscountClass() { Name = ClassName!.Trim().ToUpper(), Value = theValue }, "addcustomerdiscounttype");

        await Cancel();
    }
}
