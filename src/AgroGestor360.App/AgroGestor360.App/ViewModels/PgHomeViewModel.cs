using AgroGestor360.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class PgHomeViewModel : ObservableRecipient
{
    [RelayCommand]
    async Task GoToSettings() => await Shell.Current.GoToAsync(nameof(PgSettings), true);

    [RelayCommand]
    async Task GoToExpense() => await Shell.Current.GoToAsync(nameof(PgExpense), true);

    [RelayCommand]
    async Task GoToLoans() => await Shell.Current.GoToAsync(nameof(PgLoans), true);

    [RelayCommand]
    async Task GoToSales() => await Shell.Current.GoToAsync(nameof(PgSales), true);
}
