using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Settings.Shareholders;

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
