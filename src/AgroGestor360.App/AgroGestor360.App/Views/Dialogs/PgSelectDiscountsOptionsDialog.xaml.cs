using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Dialogs;

public partial class PgSelectDiscountsOptionsDialog : ContentPage
{
	public PgSelectDiscountsOptionsDialog(PgSelectDiscountsOptionsDialogViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
	}
}