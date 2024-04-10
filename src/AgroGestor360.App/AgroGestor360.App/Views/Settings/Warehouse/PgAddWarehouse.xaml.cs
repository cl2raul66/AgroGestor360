using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings.Warehouse;

public partial class PgAddWarehouse : ContentPage
{
	public PgAddWarehouse(PgAddWarehouseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}