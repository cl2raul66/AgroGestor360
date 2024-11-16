using AgroGestor360Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360App.ViewModels;

[QueryProperty(nameof(CancelToken), "canceltoken")]
[QueryProperty(nameof(CurrentCustomer), "currentcustomer")]
[QueryProperty(nameof(CreditItems), "credits")]
[QueryProperty(nameof(TimeLimits), "timelimits")]
[QueryProperty(nameof(DefaultTimeLimitForCredit), "defaulttimelimits")]
public partial class PgSetCreditForCustomerViewModel : ObservableValidator
{
    [ObservableProperty]
    string? cancelToken;

    [ObservableProperty]
    TimeLimitForCredit? defaultTimeLimitForCredit;

    [ObservableProperty]
    DTO5_1? currentCustomer;

    [ObservableProperty]
    LineCreditItem[]? creditItems;

    [ObservableProperty]
    [Required]
    LineCreditItem? selectedCredit;

    [ObservableProperty]
    TimeLimitForCredit[]? timeLimits;

    [ObservableProperty]
    [Required]
    TimeLimitForCredit? selectedTimeLimit;

    [ObservableProperty]
    bool isVisisbleInfo;

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", CancelToken!);
        await Shell.Current.GoToAsync("..", true);
    }

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            IsVisisbleInfo = true;
            await Task.Delay(4000);
            IsVisisbleInfo = false;
            return;
        }

        LineCredit lineCredit = new()
        {
            Id = SelectedCredit!.Id,
            Name = SelectedCredit!.Name,
            Amount = SelectedCredit!.Amount,
            TimeLimit = SelectedTimeLimit!.TimeLimit
        };

        _ = WeakReferenceMessenger.Default.Send(lineCredit, "setcreditforcustomer");

        await Shell.Current.GoToAsync("..", true);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(DefaultTimeLimitForCredit))
        {
            if (DefaultTimeLimitForCredit is not null)
            {
                SelectedTimeLimit = TimeLimits!.FirstOrDefault(x => x.Id == DefaultTimeLimitForCredit.Id);
            }
        }
    }
}
