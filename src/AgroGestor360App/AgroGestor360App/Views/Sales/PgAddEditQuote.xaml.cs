using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Sales;

public partial class PgAddEditQuote : ContentPage
{
	public PgAddEditQuote(PgAddEditQuoteViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}