using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Expense;

public partial class PgAddExpense : ContentPage
{
	public PgAddExpense(PgAddExpenseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}