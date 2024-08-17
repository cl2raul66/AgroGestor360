using AgroGestor360.App.Tools.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentDialog), "dialog")]
public partial class PgDeletedInSaleViewModel : ObservableObject
{
    [ObservableProperty]
    string? currentDialog;

    [ObservableProperty]
    bool dueOperatorError = true;

    [RelayCommand]
    async Task Save()
    {
        _ = WeakReferenceMessenger.Default.Send(DueOperatorError.ToString(), CurrentDialog!);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send(new CancelDialogForPgSalesRequestMessage(true));
        await Shell.Current.GoToAsync("..");
    }
}
