using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;
using UnitsNet;

namespace AgroGestor360App.ViewModels;

public partial class CvLineCreditsViewModel : ObservableRecipient
{
    readonly ILineCreditsService lineCreditsServ;
    readonly ITimeLimitsCreditsService timeLimitsCreditsServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvLineCreditsViewModel(ILineCreditsService lineCreditsService, ITimeLimitsCreditsService timeLimitsCreditsService, IAuthService authService)
    {
        lineCreditsServ = lineCreditsService;
        timeLimitsCreditsServ = timeLimitsCreditsService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    ObservableCollection<LineCreditItem>? credits;

    [ObservableProperty]
    LineCreditItem? selectedCredit;

    [RelayCommand]
    async Task ShowSetDefaultTimeLimit()
    {
        IsActive = true;
        await Shell.Current.GoToAsync(nameof(PgSetDefaultTimeLimit), true);
    }

    [RelayCommand]
    async Task ShowAdminTimeLimit()
    {
        await Shell.Current.GoToAsync(nameof(PgAdminTimeLimit), true);
    }

    [RelayCommand]
    async Task ShowNewCredit()
    {
        IsActive = true;
        await Shell.Current.GoToAsync(nameof(PgAddLineCredit), true);
    }

    [RelayCommand]
    async Task ShowDeletedCredit()
    {
        StringBuilder sb = new();
        sb.AppendLine("Deseas eliminar la siguiente línea de crédito:");
        sb.AppendLine($"Nombre: {SelectedCredit!.Name}");
        sb.AppendLine($"Descuento: {SelectedCredit!.Amount:C}");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar línea de crédito", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedCredit = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedCredit = null;
            return;
        }

        bool result = await lineCreditsServ.DeleteAsync(serverURL, SelectedCredit!.Id);
        if (result)
        {
            Credits!.Remove(SelectedCredit!);
        }

        SelectedCredit = null;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvLineCreditsViewModel, LineCreditItem, string>(this, "addlinecredit", async (r, m) =>
        {
            int result = await lineCreditsServ.InsertAsync(serverURL, m);
            if (result > 0)
            {
                r.Credits ??= [];
                m.Id = result;
                r.Credits.Insert(0, m);
            }

            IsActive = false;
            SelectedCredit = null;
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        IsBusy = true;
        await GetLineCredits();
        var timeLimitsCredits = await timeLimitsCreditsServ.GetAllAsync(serverURL);
        if (timeLimitsCredits is not null && timeLimitsCredits.Any())
        {

        }
        IsBusy = false;
    }

    async Task GetLineCredits()
    {
        bool exist = await lineCreditsServ.ExistAsync(serverURL);
        if (exist)
        {
            Credits = new(await lineCreditsServ.GetAllAsync(serverURL));
        }
    }
    #endregion
}
