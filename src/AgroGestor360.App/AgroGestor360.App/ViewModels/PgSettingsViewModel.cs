using AgroGestor360.App.Services;
using AgroGestor360.Client.Services;
using AgroGestor360.Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class PgSettingsViewModel : ObservableObject
{
    readonly INavigationService navigationServ;
    readonly IOrganizationService organizationServ;
    readonly IApiService apiServ;
    readonly string serverURL;

    public PgSettingsViewModel(INavigationService navigationService, IOrganizationService organizationService, IApiService apiService)
    {
        navigationServ = navigationService;
        organizationServ = organizationService;
        apiServ = apiService;
        SelectedMenu = string.Empty;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);

        apiServ.OnReceiveStatusMessage += ApiServ_OnReceiveStatusMessage;

        AppInfo = $"Versión: {VersionTracking.Default.CurrentVersion}";
        Inizialice();
    }

    [ObservableProperty]
    bool isVisibleMenu;

    [ObservableProperty]
    bool haveConnection;

    [ObservableProperty]
    string? appInfo;

    public IEnumerable<string> Menu => DeviceInfo.Idiom == DeviceIdiom.Phone
        ? ["Conexión", "Entidad"] 
        : ["Conexión",
        "Entidad",
        "Bancos",
        "Descuentos",
        "Línea de créditos",
        "Vendedores",
        "Clientes",
        "Almacén",
        "Productos"
    ];

    [ObservableProperty]
    string? selectedMenu;

    [ObservableProperty]
    ContentView? currentContent;

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    void ShowMenu()
    {
        IsVisibleMenu = !IsVisibleMenu;
    }

    void ApiServ_OnReceiveStatusMessage(ServerStatus status)
    {
        HaveConnection = status is ServerStatus.Running;
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectedMenu))
        {
            switch (SelectedMenu)
            {
                case "Conexión":
                    navigationServ.NavigateToView<CvConnectionViewModel>(view => CurrentContent = view);
                    break;
                case "Entidad":
                    SelectedMenu = null;
                    string serverURL = Preferences.Default.Get("serverurl", string.Empty);
                    var org = await organizationServ.GetOrganization(serverURL);
                    StringBuilder sb = new();
                    if (org is not null)
                    {
                        sb.AppendLine($"NOMBRE: {org.Name}");
                        sb.AppendLine($"TELEFONO: {org.Phone}");
                        sb.AppendLine($"CORREO ELECTRONICO: {org.Email}");
                        sb.AppendLine($"DIRECCION: {org.Address}");
                    }
                    else
                    {
                        sb.AppendLine("No hay información de la entidad");
                    }
                    await Shell.Current.DisplayAlert("Información de la empresa", sb.ToString(), "Cerrar");
                    break;
                case "Bancos":
                    navigationServ.NavigateToView<CvBankAccountsViewModel>(view => CurrentContent = view);
                    break;
                case "Descuentos":
                    navigationServ.NavigateToView<CvDiscountsViewModel>(view => CurrentContent = view);
                    break;
                case "Linea de créditos":
                    navigationServ.NavigateToView<CvLineCreditsViewModel>(view => CurrentContent = view);
                    break;
                case "Almacén":
                    navigationServ.NavigateToView<CvWarehouseViewModel>(view => CurrentContent = view);
                    break;
                case "Clientes":
                    navigationServ.NavigateToView<CvCustomersViewModel>(view => CurrentContent = view);
                    break;
                case "Productos":
                    navigationServ.NavigateToView<CvProductsViewModel>(view => CurrentContent = view);
                    break;
                case "Vendedores":
                    navigationServ.NavigateToView<CvSellersViewModel>(view => CurrentContent = view);
                    break;
                default:
                    navigationServ.NavigateToNullView(view => CurrentContent = view);
                    break;
            }
            if (SelectedMenu is not null)
            {
                IsVisibleMenu = false;
            }
        }
    }

    async void Inizialice()
    {
        HaveConnection = await apiServ.ConnectToServerHub(serverURL);
    }
}
