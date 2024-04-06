using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class CvWarehouse : ContentView
{
	public CvWarehouse(CvWarehouseViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
		BindingContext = vm;
	}
}