using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Sales;

public partial class PgAddEditSale : ContentPage
{
	public PgAddEditSale(PgAddEditSaleViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}