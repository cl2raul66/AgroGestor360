using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Dialogs;

public partial class PgAuthenticationDialog : ContentPage
{
	public PgAuthenticationDialog(PgAuthenticationDialogViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
	}
}