using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgAddDiscountForCustomer : ContentPage
{
	public PgAddDiscountForCustomer(PgAddDiscountForCustomerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}