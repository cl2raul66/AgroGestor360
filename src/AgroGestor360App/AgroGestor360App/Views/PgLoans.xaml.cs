using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views;

public partial class PgLoans : ContentPage
{
	public PgLoans(PgLoansViewModel vm)
	{
        InitializeComponent();

		BindingContext = vm;
	}
}