using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using vCardLib.Models;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentBank), nameof(CurrentBank))]
[QueryProperty(nameof(BankAccountNumbers), nameof(BankAccountNumbers))]
public partial class PgAddAccountOrCardViewModel : ObservableValidator
{
    readonly IFinancialInstrumentTypeService financialInstrumentTypeServ;

    public PgAddAccountOrCardViewModel(IFinancialInstrumentTypeService financialInstrumentTypeService)
    {
        financialInstrumentTypeServ = financialInstrumentTypeService;
        FinancialType = [.. financialInstrumentTypeServ.GetAll()];
    }

    [ObservableProperty]
    Bank? currentBank;

    [ObservableProperty]
    List<string>? bankAccountNumbers;

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

        if (HasErrors || string.IsNullOrEmpty(SelectedFinancialType) || BankAccountNumbers!.Contains(Number!))
        {
            IsVisibleInfo = true;
            await Task.Delay(3000);
            IsVisibleInfo = false;
            return;
        }

        BankAccount newBankAccount = new()
        {
            Alias = Alias!.Trim().ToUpper(),
            Number = Number!.Trim(),
            BankName = CurrentBank!.Name,
            InstrumentType = financialInstrumentTypeServ.GetByName(SelectedFinancialType) ?? 0,
            Beneficiary = new vCard(vCardLib.Enums.vCardVersion.v4)
            {
                Language = new Language(CultureInfo.CurrentCulture.TwoLetterISOLanguageName),
                FormattedName = BeneficiaryFullName!.Trim().ToUpper(),
                PhoneNumbers = [new TelephoneNumber(BeneficiaryPhone!.Trim(), vCardLib.Enums.TelephoneNumberType.None)],
                CustomFields = [new KeyValuePair<string, string>("NIT", BeneficiaryNIT!)],
                EmailAddresses = string.IsNullOrEmpty(BeneficiaryEMail) ? new() : [new EmailAddress(BeneficiaryEMail!.Trim().ToLower(), vCardLib.Enums.EmailAddressType.None)]
            }
        };

        WeakReferenceMessenger.Default.Send(newBankAccount, "addBankAccount");

        await Cancel();
    }
}
