using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgSetURL : ContentPage
{
	public PgSetURL(PgSetURLViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}