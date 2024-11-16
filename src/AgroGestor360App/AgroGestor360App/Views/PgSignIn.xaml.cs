using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views;

public partial class PgSignIn : ContentPage
{
	public PgSignIn(PgSignInViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}