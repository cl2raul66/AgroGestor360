namespace AgroGestor360.App;

public partial class App : Application
{
    public App()
    {
        ////Register Syncfusion license
        //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NCaF5cXmpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWXlceXRWQ2dfVU10X0I=");

        InitializeComponent();

        MainPage = new AppShell();
    }
}
