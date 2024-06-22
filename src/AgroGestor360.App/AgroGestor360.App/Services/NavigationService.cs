using AgroGestor360.App.ViewModels;
using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Settings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AgroGestor360.App.Services;

public interface INavigationService
{
    void NavigateToNullView(Action<ContentView> updateViewAction);
    void NavigateToView<TViewModel>(Action<ContentView> updateViewAction) where TViewModel : ObservableObject;
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateToNullView(Action<ContentView> updateViewAction)
    {
        updateViewAction(new CvNullSelected());
    }

    public void NavigateToView<TViewModel>(Action<ContentView> updateViewAction) where TViewModel : ObservableObject
    {
        var viewModel = _serviceProvider.GetService<TViewModel>();

        switch (viewModel)
        {
            case CvConnectionViewModel vm:
                updateViewAction(new CvConnection(vm));
                break;
            case CvUsersViewModel vm:
                updateViewAction(new CvUsers(vm));
                break;
            case CvProductsViewModel vm:
                updateViewAction(new CvProducts(vm));
                break;
            case CvBankAccountsViewModel vm:
                updateViewAction(new CvBankAccounts(vm));
                break;
            case CvDiscountsViewModel vm:
                updateViewAction(new CvDiscounts(vm));
                break;
            case CvLineCreditsViewModel vm:
                updateViewAction(new CvLineCredits(vm));
                break;
            case CvCustomersViewModel vm:
                updateViewAction(new CvCustomers(vm));
                break;
            case CvWarehouseViewModel vm:
                updateViewAction(new CvWarehouse(vm));
                break;
            case CvSellersViewModel vm:
                updateViewAction(new CvSellers(vm));
                break;
            default:
                updateViewAction(new CvNullSelected());
                break;
        }
    }
}
