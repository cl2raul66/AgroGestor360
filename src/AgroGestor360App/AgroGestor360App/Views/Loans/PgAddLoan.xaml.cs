using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Loans;

public partial class PgAddLoan : ContentPage
{
	public PgAddLoan(PgAddLoanViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}