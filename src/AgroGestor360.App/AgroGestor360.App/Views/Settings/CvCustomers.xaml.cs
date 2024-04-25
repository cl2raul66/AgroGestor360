using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.View.Settings;

public partial class CvCustomers : ContentView
{
	public CvCustomers(CvCustomersViewModel vm)
	{
		InitializeComponent();

		vm.Initialize();
        BindingContext = vm;

        CvDTO5_1.SelectionChanged += (s, e) =>
        {
            if (e.CurrentSelection.Count == 0)
            {
                CvDTO5_1.SelectionMode = SelectionMode.None;
            }
            CvDTO5_1.SelectionMode = SelectionMode.Single;
        };
    }
}