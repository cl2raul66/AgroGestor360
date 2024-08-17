using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class PgAddLineCredit : ContentPage
{
	public PgAddLineCredit(PgAddLineCreditViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}