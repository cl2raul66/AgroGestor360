using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class PgHome : ContentPage
{
	public PgHome(PgHomeViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
		BindingContext = vm;
	}
}