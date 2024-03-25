using AgroGestor360.App.ViewModels.Settings.Warehouse;

namespace AgroGestor360.App.Views.Settings.Warehouse;

public partial class PgAddMerchandise : ContentPage
{
	public PgAddMerchandise(PgAddMerchandiseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}