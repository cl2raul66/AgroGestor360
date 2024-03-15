using AgroGestor360.App.ViewModels.Settings.Customers;

namespace AgroGestor360.App.Views.Settings.Customers;

public partial class PgAddEditCustomer : ContentPage
{
	public PgAddEditCustomer(PgAddEditCustomerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}