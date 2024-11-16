using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAddDiscountForCustomer : ContentPage
{
	public PgAddDiscountForCustomer(PgAddDiscountForCustomerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}