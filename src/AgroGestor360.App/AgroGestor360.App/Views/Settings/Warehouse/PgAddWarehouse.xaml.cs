using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgAddEditWarehouse : ContentPage
{
	public PgAddEditWarehouse(PgAddEditWarehouseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}