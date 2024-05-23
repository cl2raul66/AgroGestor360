﻿using AgroGestor360.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360.App.ViewModels;

public partial class PgSignInViewModel : ObservableValidator
{
    public PgSignInViewModel()
    {
        AutoSignIn();
    }

    [RelayCommand]
    async Task Signin() => await Shell.Current.GoToAsync(nameof(PgHome), true);

    #region EXTRA
    async void AutoSignIn() => await Signin();
    #endregion
}
