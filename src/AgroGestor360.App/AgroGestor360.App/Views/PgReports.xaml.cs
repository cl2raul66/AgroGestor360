using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class PgReports : ContentPage
{
	public PgReports(PgReportsViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
        BindingContext = vm;
    }
}