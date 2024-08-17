using AgroGestor360.App.ViewModels;
using Microsoft.Maui.Controls;

namespace AgroGestor360.App.Views;

public partial class PgReports : ContentPage
{
	public PgReports(PgReportsViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
        BindingContext = vm;
    }

    private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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