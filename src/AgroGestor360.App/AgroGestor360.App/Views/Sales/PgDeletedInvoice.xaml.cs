using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Sales;

public partial class PgDeletedInvoice : ContentPage
{
	public PgDeletedInvoice(PgDeletedInvoiceViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
    }
}