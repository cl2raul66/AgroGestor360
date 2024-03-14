using AgroGestor360.App.Services;
using AgroGestor360.App.ViewModels.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360.App.ViewModels;

public partial class PgSettingsViewModel : ObservableObject
{
    readonly INavigationService navigationServ;

    public PgSettingsViewModel(INavigationService navigationService)
    {
        navigationServ = navigationService;
    }

    [ObservableProperty]
    ContentView? currentContent;

    [RelayCommand]
    async Task GoToBack() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    void ShowCvSeedCapital()
    {
        navigationServ.NavigateToView<CvSeedCapitalViewModel>(view => CurrentContent = view);
    }

    [RelayCommand]
    void ShowCvUsers()
    {
        navigationServ.NavigateToView<CvUsersViewModel>(view => CurrentContent = view);
    }

    [RelayCommand]
    void ShowCvProducts()
    {
        navigationServ.NavigateToView<CvProductsViewModel>(view => CurrentContent = view);
    }

    [RelayCommand]
    void ShowCvShareholders()
    {
        navigationServ.NavigateToView<CvShareholdersViewModel>(view => CurrentContent = view);
    }

    [RelayCommand]
    void ShowCvBankAccounts()
    {
        navigationServ.NavigateToView<CvBankAccountsViewModel>(view => CurrentContent = view);
    }
}
