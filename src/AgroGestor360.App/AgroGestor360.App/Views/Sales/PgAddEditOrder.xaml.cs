using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Sales;

public partial class PgAddEditOrder : ContentPage
{
	public PgAddEditOrder(PgAddEditOrderViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}