using AgroGestor360.App.ViewModels.Settings.Products;

namespace AgroGestor360.App.Views.Settings.Products;

public partial class PgAddItem : ContentPage
{
	public PgAddItem(PgAddItemViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}