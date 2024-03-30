using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Settings.Connection;

public partial class PgSetURLViewModel
{
    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Test()
    {
        await Cancel();
    }
    
    [RelayCommand]
    async Task Save()
    {
        await Cancel();
    }
}
