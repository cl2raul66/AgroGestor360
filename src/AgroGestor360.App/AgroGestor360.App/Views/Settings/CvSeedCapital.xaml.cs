using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class CvSeedCapital : ContentView
{
	public CvSeedCapital(CvSeedCapitalViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}