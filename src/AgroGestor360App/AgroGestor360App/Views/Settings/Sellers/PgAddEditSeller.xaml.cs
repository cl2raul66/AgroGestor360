using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAddEditSeller : ContentPage
{
	public PgAddEditSeller(PgAddEditSellerViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}