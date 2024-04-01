using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings.Connection;

public partial class PgSetURL : ContentPage
{
	public PgSetURL(PgSetURLViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}