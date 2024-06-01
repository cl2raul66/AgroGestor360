using AgroGestor360.App.Views.Settings;
using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

public partial class PgAddLineCreditViewModel : ObservableValidator
{
    [ObservableProperty]
    DateTime date;

    [ObservableProperty]
    [Required]
    [MinLength(3)]
    string? name;

    [ObservableProperty]
    [Required]
    string? amount;

    [ObservableProperty]
    bool isVisisbleInfo;

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", nameof(PgAddLineCredit));
        await Shell.Current.GoToAsync("..", true);
    }

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors || !double.TryParse(Amount, out double theAmount))
        {
            IsVisisbleInfo = true;
            await Task.Delay(4000);
            IsVisisbleInfo = false;

            return;
        }

        LineCreditItem lineCredit = new()
        {
            Name = Name!.Trim().ToUpper(),
            Amount = theAmount
        };

        _ = WeakReferenceMessenger.Default.Send(lineCredit, "addlinecredit");

        await Shell.Current.GoToAsync("..", true);
    }
}
