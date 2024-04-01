using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Title), "title")]
public partial class PgAddEditShareholderViewModel : ObservableValidator
{
    [ObservableProperty]
    string? title;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        await Cancel();
    }
}
