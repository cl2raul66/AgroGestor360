using AgroGestor360.App.ViewModels.Settings;

namespace AgroGestor360.App.Views.Settings;

public partial class CvConnection : ContentView
{
	public CvConnection(CvConnectionViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}