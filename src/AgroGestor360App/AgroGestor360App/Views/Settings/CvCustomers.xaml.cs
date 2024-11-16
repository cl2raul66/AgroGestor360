using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class CvCustomers : ContentView
{
	public CvCustomers(CvCustomersViewModel vm)
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