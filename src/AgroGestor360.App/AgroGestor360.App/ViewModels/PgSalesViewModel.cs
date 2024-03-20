using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class PgSalesViewModel : ObservableObject
{
    [ObservableProperty]
    bool isBillsVisible;

    [RelayCommand]
    void ViewPresale()
    {
        IsBillsVisible = false;
    }

    [RelayCommand]
    void ViewBills()
    {
        IsBillsVisible = true;
    }

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);
}
