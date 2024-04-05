using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.View.Settings;

public partial class CvCustomers : ContentView
{
	public CvCustomers(CvCustomersViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
		BindingContext = vm;
	}
}