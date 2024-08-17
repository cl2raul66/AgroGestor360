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
    readonly string serverURL;

    public PgAuthenticationDialogViewModel(IAuthService authService)
    {
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
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

        var result = await authServ.AuthRoot(serverURL, Pwd!);

        _ = WeakReferenceMessenger.Default.Send(result.ToString(), "AuthDlgResult");
        await Task.CompletedTask;
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Task.CompletedTask;
    }
}
