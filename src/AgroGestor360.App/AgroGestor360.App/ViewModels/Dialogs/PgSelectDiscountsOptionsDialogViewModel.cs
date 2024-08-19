using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Options), nameof(Options))]
[QueryProperty(nameof(SendToken), nameof(SendToken))]
public partial class PgSelectDiscountsOptionsDialogViewModel : ObservableObject
{
    [ObservableProperty]
    string? sendToken;

    [ObservableProperty]
    DiscountForCustomer[]? options;

    [ObservableProperty]
    DiscountForCustomer? selectedOption;

    [RelayCommand]
    async Task SetDiscontOption()
    {
        _ = WeakReferenceMessenger.Default.Send(SelectedOption!, SendToken!);
        await Shell.Current.GoToAsync("..", true);
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", "16030c9d-7d3f-11eb-9631-0242ac130002");
        await Shell.Current.GoToAsync("..", true);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Options))
        {
            if (Options is not null && Options.Length > 0)
            {
                SelectedOption = Options[0];
            }
        }
    }
}
