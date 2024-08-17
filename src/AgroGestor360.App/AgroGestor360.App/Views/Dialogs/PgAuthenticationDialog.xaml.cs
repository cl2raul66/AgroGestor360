using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Dialogs;

public partial class PgAuthenticationDialog : ContentPage
{
	public PgAuthenticationDialog(PgAuthenticationDialogViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
	}
}