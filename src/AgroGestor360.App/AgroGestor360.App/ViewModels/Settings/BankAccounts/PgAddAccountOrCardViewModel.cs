using AgroGestor360.Client.Models;
using AgroGestor360.Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using vCardLib.Models;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentBank), nameof(CurrentBank))]
public partial class PgAddAccountOrCardViewModel : ObservableValidator
{
    readonly Dictionary<int, string> FinancialInstrument = new()
    {
        {(int)FinancialInstrumentType.Current, "Cuenta corriente"},
        {(int)FinancialInstrumentType.Savings, "Cuenta de ahorros"},
        {(int)FinancialInstrumentType.Investment, "Cuenta de inversión"},
        {(int)FinancialInstrumentType.Loan, "Cuenta de préstamo"},
        {(int)FinancialInstrumentType.Payroll, "Cuenta de nómina"},
        {(int)FinancialInstrumentType.CreditCard, "Tarjeta de Crédito"},
        {(int)FinancialInstrumentType.DebitCard, "Tarjeta de Débito"}
    };

    public PgAddAccountOrCardViewModel()
    {
        FinancialType = [.. FinancialInstrument.Values];
    }

    [ObservableProperty]
    Bank? currentBank;

    [ObservableProperty]
    string? alias;

    [ObservableProperty]
    [Required]
    [MinLength(4)]
    string? number;

    [ObservableProperty]
    List<string>? financialType;

    [ObservableProperty]
    string? selectedFinancialType;

    [ObservableProperty]
    [Required]
    [MinLength(2)]
    string? beneficiaryFullName;

    [ObservableProperty]
    [Required]
    [MinLength(6)]
    string? beneficiaryNIT;

    [ObservableProperty]
    [Required]
    [Phone]
    string? beneficiaryPhone;

    [ObservableProperty]
    [EmailAddress]
    string? beneficiaryEMail;

    [ObservableProperty]
    bool isVisibleInfo;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors || string.IsNullOrEmpty(SelectedFinancialType))
        {
            IsVisibleInfo = true;
            await Task.Delay(3000);
            IsVisibleInfo = false;
            return;
        }

        int idx = FinancialInstrument.FirstOrDefault(x => x.Value == SelectedFinancialType).Key;

        BankAccount newBankAccount = new() {
            Alias = Alias!.Trim().ToUpper(),
            Number = Number!.Trim(),
            BankId = CurrentBank!.Id, 
            InstrumentType = (FinancialInstrumentType)idx, 
            Beneficiary = new vCard(vCardLib.Enums.vCardVersion.v4) { 
                FormattedName = BeneficiaryFullName!.Trim().ToUpper(), 
                Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName), 
                EmailAddresses = string.IsNullOrEmpty(BeneficiaryEMail) ? new() : [new EmailAddress(BeneficiaryEMail!.Trim().ToLower(),vCardLib.Enums.EmailAddressType.None)], 
                PhoneNumbers = [new TelephoneNumber(BeneficiaryPhone!.Trim(), vCardLib.Enums.TelephoneNumberType.None)]
            }, 
            Disabled = false };

        WeakReferenceMessenger.Default.Send(newBankAccount, "addBankAccount");

        await Cancel();
    }
}
