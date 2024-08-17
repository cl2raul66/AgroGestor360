using AgroGestor360.App.Views.Loans;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class PgLoansViewModel : ObservableObject
{
    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task ShowAddLoan() => await Shell.Current.GoToAsync(nameof(PgAddLoan), true);

    [RelayCommand]
    async Task ShowAmortization() => await Shell.Current.GoToAsync(nameof(PgAmortization), true);
}
