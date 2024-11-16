using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Sales;

public partial class PgAddEditSale : ContentPage
{
	public PgAddEditSale(PgAddEditSaleViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}