using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class CvSellers : ContentView
{
	public CvSellers(CvSellersViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
		BindingContext = vm;
	}
}