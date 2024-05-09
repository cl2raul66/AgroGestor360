using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Sales;

public partial class PgAddEditQuote : ContentPage
{
	public PgAddEditQuote(PgAddEditQuoteViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}