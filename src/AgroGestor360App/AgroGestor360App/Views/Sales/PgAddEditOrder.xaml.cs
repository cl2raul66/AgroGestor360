using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Sales;

public partial class PgAddEditOrder : ContentPage
{
	public PgAddEditOrder(PgAddEditOrderViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}