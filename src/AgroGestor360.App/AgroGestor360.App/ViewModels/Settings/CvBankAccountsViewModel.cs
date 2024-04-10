using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class CvBankAccountsViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IBanksService banksServ;
    readonly IAuthService authServ;
    readonly IBankAccountsService bankAccountsServ;

    public CvBankAccountsViewModel(IBanksService banksService, IAuthService authService, IBankAccountsService bankAccountsService)
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        banksServ = banksService;
        authServ = authService;
        bankAccountsServ = bankAccountsService;
        IsActive = true;
    }

    [ObservableProperty]
    ObservableCollection<Bank>? banks;

    [ObservableProperty]
    Bank? selectedBank;

    [ObservableProperty]
    ObservableCollection<BankAccount>? bankAccounts;

    [ObservableProperty]
    BankAccount? selectedBankAccount;

    [RelayCommand]
    async Task ShowAddAccountOrCard()
    {
        var numbers = await bankAccountsServ.GetAllNumbersAsync(serverURL);
        Dictionary<string, object> sendData = new() {
            { "CurrentBank", SelectedBank! },
            { "BankAccountNumbers",  numbers.ToList() }
        };
        await Shell.Current.GoToAsync(nameof(PgAddAccountOrCard), true, sendData);
    }

    [RelayCommand]
    async Task AddBank()
    {
        var result = await Shell.Current.DisplayPromptAsync("Agregar banco", "Nombre:", "Agregar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
        {
            return;
        }
        var name = result.Trim().ToUpper();
        if (Banks?.Any(x => x.Name == name) ?? false)
        {
            return;
        }
        var newBank = new Bank() { Name = name };
        result = await banksServ.InsertBankAsync(serverURL, newBank); 
        if (!string.IsNullOrEmpty(result))
        {
            Banks ??= [];
            newBank.Id = result;
            Banks.Insert(0, newBank);
        }
    }

    [RelayCommand]
    async Task DeleteBank()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el banco: {SelectedBank!.Name}?");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar banco", sb.ToString(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedBank = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedBank = null;
            return;
        }

        var result = await banksServ.DeleteBankAsync(serverURL, SelectedBank!.Id!);
        if (result)
        {
            Banks!.Remove(SelectedBank);
            SelectedBank = null;
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvBankAccountsViewModel, BankAccount, string>(this, "addBankAccount", async (r, m) =>
        {
            bool result = await bankAccountsServ.InsertBankAsync(serverURL, m);
            if (result)
            {
                BankAccounts ??= [];
                BankAccounts.Insert(0, m);
            }
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        await Task.WhenAll(GetBanks(), GetBankAccounts());
    }

    async Task GetBanks()
    {
        bool exist = await banksServ.CheckExistence(serverURL);
        if (exist)
        {
            var getbanks = await banksServ.GetBanksAsync(serverURL);
            Banks = new(getbanks!);
        }
    }

    async Task GetBankAccounts()
    {
        bool exist = await bankAccountsServ.CheckExistence(serverURL);
        if (exist)
        {
            var getbanks = await bankAccountsServ.GetBanksAsync(serverURL);
            BankAccounts = new(getbanks!);
        }
    }
    #endregion
}
