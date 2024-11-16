using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAddEditCustomer : ContentPage
{
	public PgAddEditCustomer(PgAddEditCustomerViewModel vm)
	{
        InitializeComponent();

        BindingContext = vm;
    }
}