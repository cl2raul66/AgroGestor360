using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views;

public partial class PgHome : ContentPage
{
	public PgHome(PgHomeViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
		BindingContext = vm;
	}
}