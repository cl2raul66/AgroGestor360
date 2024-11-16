using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Sales;

public partial class PgDeletedInSale : ContentPage
{
	public PgDeletedInSale(PgDeletedInSaleViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
    }
}