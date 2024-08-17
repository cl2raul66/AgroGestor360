using AgroGestor360.App.Tools.Messages;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentInvoice), "currentInvoice")]
public partial class PgAmortizeInvoiceCreditViewModel : ObservableValidator
{
    readonly IPaymentTypeService paymentTypeServ;
    readonly IInvoicesService invoicesServ;

    public PgAmortizeInvoiceCreditViewModel(IPaymentTypeService paymentTypeService, IInvoicesService invoicesService)
    {
        paymentTypeServ = paymentTypeService;
        invoicesServ = invoicesService;
        Date = DateTime.Now;
    }

    [ObservableProperty]
    bool isVisibleInfo;

    [ObservableProperty]
    DTO10? currentInvoice;

    [ObservableProperty]
    ObservableCollection<string>? immediateTypes;

    [ObservableProperty]
    string? selectedImmediateType;

    [ObservableProperty]
    ObservableCollection<string>? creditTypes;

    [ObservableProperty]
    string? selectedCreditType;

    [ObservableProperty]
    DateTime date;

    [ObservableProperty]
    string? referenceNo;

    [ObservableProperty]
    string? amount;

    [RelayCommand]
    async Task SetAmortize()
    {
        ValidateAllProperties();

        if (HasErrors || (!double.TryParse(Amount, out double theAmount) && theAmount < 1.00))
        {
            IsVisibleInfo = true;
            await Task.Delay(4000);
            IsVisibleInfo = false;
            return;
        }

        PaymentMethod paymentMethod = new()
        {
            Amount = theAmount,
            Date = Date,
            ReferenceNumber = ReferenceNo,
            Type = paymentTypeServ.GetByName(SelectedCreditType!)
        };
        
        DTO10_2 dTO = new()
        {
            Code = CurrentInvoice!.Code,
            PaymentMethods = [paymentMethod]
        };

        _ = WeakReferenceMessenger.Default.Send(dTO, "setdepreciation");
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send(new CancelDialogForPgSalesRequestMessage(true));
        await Shell.Current.GoToAsync("..");
    }
}
