using Microsoft.Extensions.Configuration;
using Syncfusion.Licensing;

namespace AgroGestor360App;

public partial class App : Application
{
    public App()
    {
        var builder = new ConfigurationBuilder().AddUserSecrets<App>(true);
        IConfiguration configuration = builder.Build();
        var secretValue = configuration["SF"]; 
        SyncfusionLicenseProvider.RegisterLicense(secretValue);

        InitializeComponent();

        MainPage = new AppShell();
    }
}
