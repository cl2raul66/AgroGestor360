﻿using AgroGestor360.App.View.Settings;
using AgroGestor360.App.ViewModels.Settings;
using AgroGestor360.App.Views.Settings;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AgroGestor360.App.Services;

public interface INavigationService
{
    void NavigateToView<TViewModel>(Action<ContentView> updateViewAction) where TViewModel : ObservableObject;
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateToView<TViewModel>(Action<ContentView> updateViewAction) where TViewModel : ObservableObject
    {
        var viewModel = _serviceProvider.GetService<TViewModel>();

        switch (viewModel)
        {
            case CvSeedCapitalViewModel vm:
                updateViewAction(new CvSeedCapital(vm));
                break;
            case CvUsersViewModel vm:
                updateViewAction(new CvUsers(vm));
                break;
            case CvProductsViewModel vm:
                updateViewAction(new CvProducts(vm));
                break;
            case CvShareholdersViewModel vm:
                updateViewAction(new CvShareholders(vm));
                break;
            case CvBankAccountsViewModel vm:
                updateViewAction(new CvBankAccounts(vm));
                break;
            case CvCustomersViewModel vm:
                updateViewAction(new CvCustomers(vm));
                break;
            case CvWarehouseViewModel vm:
                updateViewAction(new CvWarehouse(vm));
                break;
            case CvSalesViewModel vm:
                updateViewAction(new CvSales(vm));
                break;
            default:
                break;
        }
    }
}
