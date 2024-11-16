using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Dialogs;

public partial class PgSelectDiscountsOptionsDialog : ContentPage
{
	public PgSelectDiscountsOptionsDialog(PgSelectDiscountsOptionsDialogViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
	}
}