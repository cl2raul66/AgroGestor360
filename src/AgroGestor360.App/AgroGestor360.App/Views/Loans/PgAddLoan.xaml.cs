using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Loans;

public partial class PgAddLoan : ContentPage
{
	public PgAddLoan(PgAddLoanViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}