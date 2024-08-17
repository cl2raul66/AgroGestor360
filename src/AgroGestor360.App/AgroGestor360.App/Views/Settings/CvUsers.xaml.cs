using AgroGestor360.App.ViewModels;

namespace AgroGestor360.App.Views.Settings;

public partial class CvUsers : ContentView
{
    public CvUsers(CvUsersViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}