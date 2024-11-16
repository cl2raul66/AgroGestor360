using AgroGestor360App.ViewModels;

namespace AgroGestor360App.Views.Settings;

public partial class CvUsers : ContentView
{
    public CvUsers(CvUsersViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}