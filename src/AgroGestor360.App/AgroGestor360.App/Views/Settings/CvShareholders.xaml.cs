using AgroGestor360.App.ViewModels.Settings;

namespace AgroGestor360.App.Views.Settings;

public partial class CvShareholders : ContentView
{
	public CvShareholders(CvShareholdersViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}