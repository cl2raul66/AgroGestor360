using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Sales;

public partial class PgAmortizeInvoiceCredit : ContentPage
{
	public PgAmortizeInvoiceCredit(PgAmortizeInvoiceCreditViewModel vm)
	{
		InitializeComponent();

        BindingContext = vm;
    }
}