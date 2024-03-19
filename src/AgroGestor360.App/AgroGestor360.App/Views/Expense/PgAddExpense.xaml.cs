using AgroGestor360.App.ViewModels.Expense;

namespace AgroGestor360.App.Views.Expense;

public partial class PgAddExpense : ContentPage
{
	public PgAddExpense(PgAddExpenseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}