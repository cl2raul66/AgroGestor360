using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings.Products;

public partial class PgCreateOffer : ContentPage
{
	public PgCreateOffer(PgCreateOfferViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}