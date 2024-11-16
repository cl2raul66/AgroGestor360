using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AgroGestor360App.ViewModels;

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
        _ = WeakReferenceMessenger.Default.Send("cancel", "A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
        await Shell.Current.GoToAsync("..", true);
    }
}
