using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Loans;

public partial class PgAmortization : ContentPage
{
	public PgAmortization(PgAmortizationViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}