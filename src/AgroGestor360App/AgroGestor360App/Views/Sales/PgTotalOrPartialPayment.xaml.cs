using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Sales;

public partial class PgTotalOrPartialPayment : ContentPage
{
	public PgTotalOrPartialPayment(PgTotalOrPartialPaymentViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}