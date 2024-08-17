// Ignore Spelling: auth

using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

public partial class PgAuthenticationDialogViewModel : ObservableValidator
{
    readonly IAuthService authServ;

    public PgAuthenticationDialogViewModel(IAuthService authService)
    {
        authServ = authService;
    }

    [ObservableProperty]
    [Required]
    string? pwd;

    [RelayCommand]
    async Task SetAndCheckPassword()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            Pwd = null;
            return;
        }

        var result = 

        _ = WeakReferenceMessenger.Default.Send(WeakReferenceMessenger.Default);
        await Task.CompletedTask;
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Task.CompletedTask;
    }
}
