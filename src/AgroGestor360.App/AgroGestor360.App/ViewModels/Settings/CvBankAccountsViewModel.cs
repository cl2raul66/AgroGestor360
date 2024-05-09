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
    readonly IFinancialInstrumentTypeService financialInstrumentTypeServ;

    public CvBankAccountsViewModel(IAuthService authService, IBankAccountsService bankAccountsService, IFinancialInstrumentTypeService financialInstrumentTypeService)
    {
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        authServ = authService;
        bankAccountsServ = bankAccountsService;
        financialInstrumentTypeServ = financialInstrumentTypeService;
    }

    [ObservableProperty]
    bool isBusy;

    #region BANKS
    [ObservableProperty]
    ObservableCollection<string>? banks;

    [ObservableProperty]
    string? selectedBank;

    [RelayCommand]
    async Task AddBank()
    {
        var result = await Shell.Current.DisplayPromptAsync("Agregar banco", "Nombre del banco:", "Agregar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
        {
            SelectedBank = null;
            SelectedBankAccount = null;
            return;
        }
        var name = result.Trim().ToUpper();
        if (Banks?.Any(x => x == name) ?? false)
        {
            SelectedBank = null;
            SelectedBankAccount = null;
            return;
        }
        Banks ??= [];
        Banks.Insert(0, name);
        string json = JsonSerializer.Serialize(Banks);
        Preferences.Default.Set("banks", json);

        SelectedBank = null;
        SelectedBankAccount = null;
    }

    [RelayCommand]
    async Task DeleteBank()
    {
        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar el banco: {SelectedBank!}?");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar banco", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedBank = null;
            SelectedBankAccount = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedBank = null;
            SelectedBankAccount = null;
            return;
        }
        bool result = Banks!.Remove(SelectedBank!);
        if (result)
        {
            string json = JsonSerializer.Serialize(Banks);
            Preferences.Default.Set("banks", json);
        }
    }
    #endregion

    #region BANK ACCOUNTS
    [ObservableProperty]
    ObservableCollection<BankAccount>? bankAccounts;

    [ObservableProperty]
    BankAccount? selectedBankAccount;

    [RelayCommand]
    async Task ShowAddAccountOrCard()
    {
        IsActive = true;
        Dictionary<string, object> sendData = new() { { "CurrentBank", SelectedBank! } };
        await Shell.Current.GoToAsync(nameof(PgAddAccountOrCard), true, sendData);
    }

    [RelayCommand]
    async Task DeleteBankAccount()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Seguro que quiere eliminar a :");
        sb.AppendLine($"Banco: {SelectedBankAccount!.BankName}");
        sb.AppendLine($"Número: {SelectedBankAccount!.Number}");
        sb.AppendLine($"Tipo: {financialInstrumentTypeServ.GetNameByType(SelectedBankAccount!.InstrumentType)}"); 
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar cuenta", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedBank = null;
            SelectedBankAccount = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedBank = null;
            SelectedBankAccount = null;
            return;
        }
        bool result = await bankAccountsServ.DeleteAsync(serverURL, SelectedBankAccount!.Number!);
        if (result)
        {
            BankAccounts!.Remove(SelectedBankAccount!);
        }

        SelectedBank = null;
        SelectedBankAccount = null;
    }
    #endregion

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvBankAccountsViewModel, BankAccount, string>(this, "addBankAccount", async (r, m) =>
        {
            bool result = await bankAccountsServ.InsertAsync(serverURL, m);
            if (result)
            {
                r.BankAccounts ??= [];
                r.BankAccounts.Insert(0, m);
            }

            IsActive = false;
            r.SelectedBank = null;
            r.SelectedBankAccount = null;
        });

        WeakReferenceMessenger.Default.Register<CvBankAccountsViewModel, string, string>(this, nameof(PgAddAccountOrCard), (r, m) =>
        {
            if (m == "cancel")
            {
                r.SelectedBank = null;
                r.SelectedBankAccount = null;
                IsActive = false;
            }
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        IsBusy = true;
        await Task.WhenAll(GetBanks(), GetBankAccounts());
        IsBusy = false;
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
