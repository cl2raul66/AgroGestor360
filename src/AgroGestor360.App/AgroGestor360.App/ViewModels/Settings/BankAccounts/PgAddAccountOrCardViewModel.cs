using AgroGestor360.App.Views.Settings.BankAccounts;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentBank), nameof(CurrentBank))]
public partial class PgAddAccountOrCardViewModel : ObservableValidator
{
    readonly IFinancialInstrumentTypeService financialInstrumentTypeServ;

    public PgAddAccountOrCardViewModel(IFinancialInstrumentTypeService financialInstrumentTypeService)
    {
        financialInstrumentTypeServ = financialInstrumentTypeService;
        FinancialType = [.. financialInstrumentTypeServ.GetAll()];
    }

    [ObservableProperty]
    string? currentBank;

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
    bool isVisibleInfo;

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", nameof(PgAddAccountOrCard));

        await Shell.Current.GoToAsync("..", true);
    }

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

        BankAccount newBankAccount = new()
        {
            Alias = Alias?.Trim().ToUpper() ?? string.Empty,
            Number = Number!.Trim(),
            BankName = CurrentBank!,
            InstrumentType = financialInstrumentTypeServ.GetByName(SelectedFinancialType) ?? 0
        };

        _ = WeakReferenceMessenger.Default.Send(newBankAccount, "addBankAccount");

        await Shell.Current.GoToAsync("..", true);
    }
}
