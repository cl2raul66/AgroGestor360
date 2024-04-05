using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgAddEditSeller : ContentPage
{
	public PgAddEditSeller(PgAddEditSellerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}