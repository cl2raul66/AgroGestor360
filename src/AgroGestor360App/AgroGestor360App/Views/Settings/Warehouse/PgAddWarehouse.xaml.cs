using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAddEditWarehouse : ContentPage
{
	public PgAddEditWarehouse(PgAddEditWarehouseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}