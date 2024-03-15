using AgroGestor360.App.ViewModels.Settings.Products;

namespace AgroGestor360.App.Views.Settings.Products;

public partial class PgAddProduct : ContentPage
{
    public PgAddProduct(PgAddProductViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}