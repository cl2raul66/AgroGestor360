using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views;

public partial class PgExpense : ContentPage
{
	public PgExpense(PgExpenseViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}