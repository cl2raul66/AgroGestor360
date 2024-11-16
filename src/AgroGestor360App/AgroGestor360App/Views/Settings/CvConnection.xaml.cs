using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class CvConnection : ContentView
{
	public CvConnection(CvConnectionViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}