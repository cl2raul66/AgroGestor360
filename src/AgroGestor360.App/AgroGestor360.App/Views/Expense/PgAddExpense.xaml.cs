using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Expense;

public partial class PgAddExpense : ContentPage
{
	public PgAddExpense(PgAddExpenseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}