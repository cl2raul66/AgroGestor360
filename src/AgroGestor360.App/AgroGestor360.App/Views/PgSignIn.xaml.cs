using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class PgSignIn : ContentPage
{
	public PgSignIn(PgSignInViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}