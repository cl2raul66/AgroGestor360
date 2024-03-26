using AgroGestor360.App.ViewModels.Settings.Sales;

namespace AgroGestor360.App.Views.Settings.Sales;

public partial class PgAddCustomerDiscountType : ContentPage
{
	public PgAddCustomerDiscountType(PgAddCustomerDiscountTypeViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}