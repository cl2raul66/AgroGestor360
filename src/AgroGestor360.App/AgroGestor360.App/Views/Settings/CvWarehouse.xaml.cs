using AgroGestor360.App.ViewModels.Settings;

namespace AgroGestor360.App.Views.Settings;

public partial class CvWarehouse : ContentView
{
	public CvWarehouse(CvWarehouseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}