using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class PgAddProduct : ContentPage
{
    public PgAddProduct(PgAddProductViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}