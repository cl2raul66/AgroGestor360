using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace AgroGestor360.App.ViewModels;

public partial class CvBankAccountsViewModel : ObservableRecipient
{
    readonly string serverURL;
    readonly IAuthService authServ;
    readonly IBankAccountsService bankAccountsServ;

    public CvBankAccountsViewModel(IAuthService authService, IBankAccountsService bankAccountsService)
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        authServ = authService;
        bankAccountsServ = bankAccountsService;
        IsActive = true;
    }

    [ObservableProperty]
    ObservableCollection<string>? banks;

    [ObservableProperty]
    string? selectedBank;

    [ObservableProperty]
    ObservableCollection<BankAccount>? bankAccounts;

    [ObservableProperty]
    BankAccount? selectedBankAccount;

    [RelayCommand]
    async Task ShowAddAccountOrCard()
    {
        Dictionary<string, object> sendData = new() {
            { "CurrentBank", SelectedBank! }
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
        if (Banks?.Any(x => x == name) ?? false)
        {
            return;
        }
        Banks ??= [];
        Banks.Insert(0, name);
        string json = JsonSerializer.Serialize(Banks);
        Preferences.Default.Set("banks", json);
    }

    [RelayCommand]
    async Task DeleteBank()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el banco: {SelectedBank!}?");
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
        bool result = Banks!.Remove(SelectedBank!);
        if (result)
        {
            string json = JsonSerializer.Serialize(Banks);
            Preferences.Default.Set("banks", json);
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvBankAccountsViewModel, BankAccount, string>(this, "addBankAccount", async (r, m) =>
        {
            bool result = await bankAccountsServ.InsertAsync(serverURL, m);
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
        string json = Preferences.Default.Get("banks", string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            Banks ??= [];
            Banks = new(JsonSerializer.Deserialize<IEnumerable<string>>(json) ?? []);
        }
        await Task.CompletedTask;
    }

    async Task GetBankAccounts()
    {
        bool exist = await bankAccountsServ.ExistAsync(serverURL);
        if (exist)
        {
            var getbanks = await bankAccountsServ.GetAllAsync(serverURL);
            BankAccounts = new(getbanks!);
        }
    }
    #endregion
}
