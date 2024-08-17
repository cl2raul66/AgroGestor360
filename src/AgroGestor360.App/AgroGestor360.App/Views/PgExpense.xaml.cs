using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class PgExpense : ContentPage
{
	public PgExpense(PgExpenseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}