using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class PgLoans : ContentPage
{
	public PgLoans(PgLoansViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}