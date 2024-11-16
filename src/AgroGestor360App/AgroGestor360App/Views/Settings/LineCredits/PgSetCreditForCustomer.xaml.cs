using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgSetCreditForCustomer : ContentPage
{
	public PgSetCreditForCustomer(PgSetCreditForCustomerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}