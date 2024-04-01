using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace AgroGestor360.App.ViewModels;

public partial class CvBankAccountsViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IBanksService banksServ;

    public CvBankAccountsViewModel(IBanksService banksService)
    {
        IsActive = true;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        banksServ = banksService;
        GetBancks();
    }

    [ObservableProperty]
    ObservableCollection<Bank>? banks;

    [ObservableProperty]
    Bank? selectedBank;

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

    protected override void OnActivated()
    {
        base.OnActivated();
    }

    #region EXTRA
    private async void GetBancks()
    {
        bool exist = await banksServ.CheckExistence(serverURL);
        if (exist)
        {
            var getbanks = await banksServ.GetBanksAsync(serverURL);
            Banks = new(getbanks!);
        }
    }
    #endregion
}
