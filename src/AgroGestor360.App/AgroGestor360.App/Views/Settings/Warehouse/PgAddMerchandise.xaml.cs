using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings.Warehouse;

public partial class PgAddMerchandise : ContentPage
{
	public PgAddMerchandise(PgAddMerchandiseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}