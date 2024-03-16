using AgroGestor360.App.ViewModels.Loans;

namespace AgroGestor360.App.Views.Loans;

public partial class PgAmortization : ContentPage
{
	public PgAmortization(PgAmortizationViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}