using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class CvWarehouse : ContentView
{
	public CvWarehouse(CvWarehouseViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
        BindingContext = vm;

        CvDTO2.SelectionChanged += (s, e) =>
        {
            if (e.CurrentSelection.Count == 0)
            {
                CvDTO2.SelectionMode = SelectionMode.None;
            }
            CvDTO2.SelectionMode = SelectionMode.Single;
        };
    }
}