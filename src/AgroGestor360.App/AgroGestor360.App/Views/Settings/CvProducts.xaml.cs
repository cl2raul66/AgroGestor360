using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class CvProducts : ContentView
{
	public CvProducts(CvProductsViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}