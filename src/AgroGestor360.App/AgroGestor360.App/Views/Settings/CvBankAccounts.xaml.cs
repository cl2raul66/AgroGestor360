using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class CvBankAccounts : ContentView
{
	public CvBankAccounts(CvBankAccountsViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}