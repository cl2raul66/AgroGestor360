using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgAddEditCustomer : ContentPage
{
	public PgAddEditCustomer(PgAddEditCustomerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
    }
}