using AgroGestor360.App.ViewModels.Settings;

namespace AgroGestor360.App.Views.Settings;

public partial class CvSales : ContentView
{
	public CvSales(CvSalesViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}