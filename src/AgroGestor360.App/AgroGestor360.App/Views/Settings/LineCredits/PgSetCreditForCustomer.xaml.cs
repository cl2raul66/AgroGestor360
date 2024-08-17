using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgSetCreditForCustomer : ContentPage
{
	public PgSetCreditForCustomer(PgSetCreditForCustomerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}