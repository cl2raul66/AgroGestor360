using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings.BankAccounts;

public partial class PgAddAccountOrCard : ContentPage
{
	public PgAddAccountOrCard(PgAddAccountOrCardViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}