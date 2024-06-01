using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgSetURL : ContentPage
{
	public PgSetURL(PgSetURLViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}