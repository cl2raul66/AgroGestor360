using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAddLineCredit : ContentPage
{
	public PgAddLineCredit(PgAddLineCreditViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}