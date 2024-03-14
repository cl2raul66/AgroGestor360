using AgroGestor360.App.ViewModels.Settings.Shareholders;

namespace AgroGestor360.App.Views.Settings.Shareholders;

public partial class PgAddEditShareholder : ContentPage
{
	public PgAddEditShareholder(PgAddEditShareholderViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}