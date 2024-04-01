using AgroGestor360.App.Views.Settings.Sales;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class CvSalesViewModel : ObservableObject
{
    [RelayCommand]
    async Task ShowAddAccountOrCard() => await Shell.Current.GoToAsync(nameof(PgAddCustomerDiscountType), true);
}
