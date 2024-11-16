using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAdminTimeLimit : ContentPage
{
	public PgAdminTimeLimit(PgAdminTimeLimitViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}