namespace AgroGestor360.App;

public partial class App : Application
{
    public App()
    {
        ////Register Syncfusion license
        //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("key");

        InitializeComponent();

        MainPage = new AppShell();
    }
}
