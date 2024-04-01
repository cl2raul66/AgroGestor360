using AgroGestor360.App.Services;
using AgroGestor360.App.ViewModels;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class PgSettingsViewModel : ObservableObject
{
    readonly INavigationService navigationServ;
    readonly IApiService apiServ;
    readonly IOrganizationService organizationServ;
    readonly string serverURL;

    public PgSettingsViewModel(INavigationService navigationService, IApiService apiService, IOrganizationService organizationService)
    {
        navigationServ = navigationService;
        apiServ = apiService;
        organizationServ = organizationService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        SelectedMenu = string.Empty;
    }

    [ObservableProperty]
    ContentView? currentContent;

    [ObservableProperty]
    string? selectedMenu;

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

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
                case "Capital inicial":
                    navigationServ.NavigateToView<CvSeedCapitalViewModel>(view => CurrentContent = view);
                    break;
                case "Cuentas y tarjetas":
                    navigationServ.NavigateToView<CvBankAccountsViewModel>(view => CurrentContent = view);
                    break;
                case "Accionistas":
                    navigationServ.NavigateToView<CvShareholdersViewModel>(view => CurrentContent = view);
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
                    break;
                case "Ventas":
                    navigationServ.NavigateToView<CvSalesViewModel>(view => CurrentContent = view);
                    break;
                default:
                    navigationServ.NavigateToNullView(view => CurrentContent = view);
                    break;
            }
        }
    }
}
