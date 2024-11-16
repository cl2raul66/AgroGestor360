using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360App.ViewModels;

public partial class PgAddLoanViewModel : ObservableValidator
{
    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        await Cancel();
    }
}
