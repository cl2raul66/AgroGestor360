using AgroGestor360App.Views.Expense;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360App.ViewModels;

public partial class PgExpenseViewModel : ObservableObject
{
    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task ShowAddExpense() => await Shell.Current.GoToAsync(nameof(PgAddExpense), true);
}
