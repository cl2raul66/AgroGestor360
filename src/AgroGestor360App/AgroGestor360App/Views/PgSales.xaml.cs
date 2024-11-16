using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views;

public partial class PgSales : ContentPage
{
	public PgSales(PgSalesViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
        BindingContext = vm;
	}

    void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var collectionView = sender as CollectionView;
        if (collectionView is not null)
        {
            if (e.CurrentSelection.Count == 0)
            {
                collectionView.SelectionMode = SelectionMode.None;
            }
            collectionView.SelectionMode = SelectionMode.Single;
        }
    }
}