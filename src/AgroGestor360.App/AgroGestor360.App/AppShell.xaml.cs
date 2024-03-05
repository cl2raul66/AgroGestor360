using AgroGestor360.App.Views;

namespace AgroGestor360.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(PgSignUp), typeof(PgSignUp));
        Routing.RegisterRoute(nameof(PgHome), typeof(PgHome));
    }
}
