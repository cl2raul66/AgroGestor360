using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAddAccountOrCard : ContentPage
{
	public PgAddAccountOrCard(PgAddAccountOrCardViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}