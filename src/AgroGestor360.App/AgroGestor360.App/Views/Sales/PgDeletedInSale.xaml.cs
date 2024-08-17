using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Sales;

public partial class PgDeletedInSale : ContentPage
{
	public PgDeletedInSale(PgDeletedInSaleViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
    }
}