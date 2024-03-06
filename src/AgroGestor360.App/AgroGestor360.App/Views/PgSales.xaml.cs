using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class PgSales : ContentPage
{
	public PgSales(PgSalesViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}