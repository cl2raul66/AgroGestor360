﻿using AgroGestor360.App.ViewModels.Settings;
using AgroGestor360.App.Views.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        // Agrega aquí más condiciones para otros ViewModels
    }
}