using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgCreateOffer : ContentPage
{
	public PgCreateOffer(PgCreateOfferViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}