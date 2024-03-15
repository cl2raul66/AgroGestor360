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

        if (viewModel is CvSeedCapitalViewModel)
        {
            var vm = viewModel as CvSeedCapitalViewModel;
            var view = new CvSeedCapital(vm!);
            updateViewAction(view);
        }
        
        if (viewModel is CvUsersViewModel)
        {
            var vm = viewModel as CvUsersViewModel;
            var view = new CvUsers(vm!);
            updateViewAction(view);
        }

        if (viewModel is CvProductsViewModel)
        {
            var vm = viewModel as CvProductsViewModel;
            var view = new CvProducts(vm!);
            updateViewAction(view);
        }

        if (viewModel is CvShareholdersViewModel)
        {
            var vm = viewModel as CvShareholdersViewModel;
            var view = new CvShareholders(vm!);
            updateViewAction(view);
        }

        if (viewModel is CvBankAccountsViewModel)
        {
            var vm = viewModel as CvBankAccountsViewModel;
            var view = new CvBankAccounts(vm!);
            updateViewAction(view);
        }

        if (viewModel is CvCustomersViewModel)
        {
            var vm = viewModel as CvCustomersViewModel;
            var view = new CvCustomers(vm!);
            updateViewAction(view);
        }
    }
}
