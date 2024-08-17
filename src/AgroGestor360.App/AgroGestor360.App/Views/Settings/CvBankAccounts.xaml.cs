using AgroGestor360.App.ViewModels;
using AgroGestor360.Client.Models;

namespace AgroGestor360.App.Views.Settings;

public partial class CvBankAccounts : ContentView
{
    public CvBankAccounts(CvBankAccountsViewModel vm)
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
    //todo: aqui esta el modo de deseleccionar un item del collectionview
}