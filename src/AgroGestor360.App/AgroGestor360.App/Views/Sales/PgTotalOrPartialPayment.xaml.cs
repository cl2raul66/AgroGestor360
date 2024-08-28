using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Sales;

public partial class PgTotalOrPartialPayment : ContentPage
{
	public PgTotalOrPartialPayment(PgTotalOrPartialPaymentViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}