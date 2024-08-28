using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(SendToken), nameof(SendToken))]
[QueryProperty(nameof(CurrentSale), nameof(CurrentSale))]
public partial class PgTotalOrPartialPaymentViewModel : ObservableObject
{
    readonly IPaymentTypeService paymentTypeServ;

    public PgTotalOrPartialPaymentViewModel(IPaymentTypeService paymentTypeService)
    {
        paymentTypeServ = paymentTypeService;
        PaymentDate = DateTime.Now;
    }

    [ObservableProperty]
    string? sendToken;

    [ObservableProperty]
    DTO10? currentSale;

    [ObservableProperty]
    string? title;

    [ObservableProperty]
    DateTime? paymentDate;

    [ObservableProperty]
    double debt;

    //[ObservableProperty]
    //DateTime? completePaymentDate;

    [ObservableProperty]
    string? amountPay;

    [ObservableProperty]
    string? referentNo;

    [ObservableProperty]
    string[]? paymentsTypes;

    [ObservableProperty]
    string? selectedPaymentType;

    [ObservableProperty]
    string? textInfo;

    [RelayCommand]
    async Task SetPay()
    {
        if (!double.TryParse(AmountPay, out double theAmountPay))
        {
            theAmountPay = 0;
        }

        if (string.IsNullOrEmpty(SelectedPaymentType))
        {
            TextInfo = "Complete los requeridos (*).";
            await Task.Delay(4000);
            TextInfo = null;

            return;
        }

        if (CurrentSale!.DaysRemaining > -1 && theAmountPay >= Debt)
        {
            TextInfo = "Debe ingresar un monto menor que el monto a pagar.";
            await Task.Delay(4000);
            TextInfo = null;

            return;
        }

        DTO10_2 data = new()
        {
            Code = CurrentSale!.Code,
            PaymentMethod = new() { Date = DateTime.Now, Amount = theAmountPay, ReferenceNumber = ReferentNo, Type = paymentTypeServ.GetByName(SelectedPaymentType!) }
        };

        _ = WeakReferenceMessenger.Default.Send(data, SendToken!);
        await Shell.Current.GoToAsync("..", true);
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", "A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
        await Shell.Current.GoToAsync("..", true);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(CurrentSale))
        {
            if (CurrentSale is not null)
            {
                PaymentsTypes = [.. paymentTypeServ.GetAll()];
                Debt = CurrentSale.TotalAmount - CurrentSale.Paid;
                if (CurrentSale.DaysRemaining == -1)
                {                    
                    AmountPay = Debt.ToString();
                    Title = "Pago total";
                }
                else
                {
                    
                    Title = "Abonar a crédito";
                }
            }
        }

        if (e.PropertyName == nameof(SendToken))
        {
            if (!string.IsNullOrEmpty(SendToken))
            {
                if (SendToken == "E7F8G9H0-I1J2-3K4L-M5N6-O7P8Q9R0S1T2")
                {
                    
                }

                if (SendToken == "F4E5D6C7-B8A9-0B1C-D2E3-F4567890ABCD")
                {
                    
                }
            }
        }
    }
}
