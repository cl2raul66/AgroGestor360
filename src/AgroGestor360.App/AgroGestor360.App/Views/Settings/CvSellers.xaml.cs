using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class CvSellers : ContentView
{
	public CvSellers(CvSellersViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
		BindingContext = vm;

		CvDTO6.SelectionChanged += (s, e) =>
        {
			if (e.CurrentSelection.Count == 0)
			{
                CvDTO6.SelectionMode = SelectionMode.None;
            }
            CvDTO6.SelectionMode = SelectionMode.Single;
        };
		//todo: aqui esta el modo de deseleccionar un item del collectionview
	}
}