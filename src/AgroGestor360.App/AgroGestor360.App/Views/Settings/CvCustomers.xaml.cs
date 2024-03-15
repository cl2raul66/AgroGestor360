using AgroGestor360.App.ViewModels.Settings;

namespace AgroGestor360.App.View.Settings;

public partial class CvCustomers : ContentView
{
	public CvCustomers(CvCustomersViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}