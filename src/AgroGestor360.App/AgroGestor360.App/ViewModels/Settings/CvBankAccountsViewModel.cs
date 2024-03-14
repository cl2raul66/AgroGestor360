using AgroGestor360.App.Views.Settings.BankAccounts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Settings;

public partial class CvBankAccountsViewModel : ObservableObject
{
    [RelayCommand]
    async Task ShowAddAccountOrCard() => await Shell.Current.GoToAsync(nameof(PgAddAccountOrCard), true);
    
    [RelayCommand]
    async Task AddBank(){
        string resul = await Shell.Current.DisplayPromptAsync("Agregar banco", "Nombre:", "Agregar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(resul))
        {
            return;
        }
    }
}
