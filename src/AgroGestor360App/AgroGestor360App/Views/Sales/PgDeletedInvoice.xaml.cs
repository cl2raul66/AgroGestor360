using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Sales;

public partial class PgDeletedInvoice : ContentPage
{
	public PgDeletedInvoice(PgDeletedInvoiceViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
    }
}