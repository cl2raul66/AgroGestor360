using AgroGestor360.App.Services;
using AgroGestor360.App.ViewModels.Settings;
using AgroGestor360.App.Views.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace AgroGestor360.App.ViewModels;

public partial class PgSettingsViewModel : ObservableObject
{
    readonly INavigationService navigationServ;

    public PgSettingsViewModel(INavigationService navigationService)
    {
        navigationServ = navigationService;
        SelectedMenu = string.Empty;
    }

    [ObservableProperty]
    ContentView? currentContent;

    [ObservableProperty]
    string? selectedMenu;

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
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
