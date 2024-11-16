using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class CvSales : ContentView
{
	public CvSales(CvSalesViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}