using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgAdminTimeLimit : ContentPage
{
	public PgAdminTimeLimit(PgAdminTimeLimitViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}