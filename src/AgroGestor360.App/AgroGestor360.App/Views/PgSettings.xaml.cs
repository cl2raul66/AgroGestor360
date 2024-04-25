using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views;

public partial class PgSettings : ContentPage
{
	public PgSettings(PgSettingsViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;

		CvOptions.SelectionChanged += (sender, e) =>
		{
            if (e.CurrentSelection.Count == 0)
            {
                CvOptions.SelectionMode = SelectionMode.None;
            }
            CvOptions.SelectionMode = SelectionMode.Single;
        };
    }
}