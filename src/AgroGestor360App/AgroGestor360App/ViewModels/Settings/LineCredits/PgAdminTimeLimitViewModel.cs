using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AgroGestor360App.ViewModels;

public partial class PgAdminTimeLimitViewModel : ObservableValidator
{
    readonly ITimeLimitsCreditsService timeLimitsCreditsServ;
    readonly string serverURL;

    public PgAdminTimeLimitViewModel(ITimeLimitsCreditsService timeLimitsCreditsService)
    {
        timeLimitsCreditsServ = timeLimitsCreditsService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        Initialize();
    }

    [ObservableProperty]
    ObservableCollection<TimeLimitForCredit>? times;

    [ObservableProperty]
    TimeLimitForCredit? selectedTime;

    [ObservableProperty]
    string? timesView;

    [ObservableProperty]
    string? newTime;

    [ObservableProperty]
    string? textInfo;

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..", true);
    }

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors || (!int.TryParse(NewTime, out int theNewTime) && theNewTime > 0))
        {
            TextInfo = " Rellene toda la información los requeridos (*)";
            await Task.Delay(5000);
            TextInfo = null;

            return;
        }

        TimeLimitForCredit timeLimitForCredit = new()
        {
            TimeLimit = theNewTime
        };

        var resul = await timeLimitsCreditsServ.InsertAsync(serverURL, timeLimitForCredit);
        if (resul > 0)
        {
            Times ??= [];
            timeLimitForCredit.Id = resul;
            if (Times.Any())
            {
                Times.Insert(0, timeLimitForCredit);
            }
            else
            {
                Times.Add(timeLimitForCredit);
            }
        }
        SetTimesView();

        NewTime = null;
    }

    [RelayCommand]
    async Task SetDefaultTimeLimit()
    {
        if (SelectedTime is null)
        {
            TextInfo = " Seleccione tiempo de crédito predeterminado";
            await Task.Delay(4000);
            TextInfo = null;

            return;
        }
        var result = await timeLimitsCreditsServ.SetDefaultAsync(serverURL, SelectedTime);
        if (result)
        {
            TextInfo = $" Se estableció {SelectedTime.TimeLimit} como predeterminado";
            await Task.Delay(4000);
            TextInfo = null;
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Times))
        {
            SetTimesView();
        }
    }

    #region EXTRA
    async void Initialize()
    {
        Times ??= [];
        Times = new(await timeLimitsCreditsServ.GetAllAsync(serverURL));
        var defacult = await timeLimitsCreditsServ.GetDefaultAsync(serverURL);
        if (defacult is not null)
        {
            SelectedTime = Times.FirstOrDefault(x => x.Id == defacult.Id);
        }
    }

    void SetTimesView()
    {
        TimesView = Times is not null && Times.Any() ? string.Join(", ", Times.Select(x => x.TimeLimit)).TrimEnd([' ', ',']) : "No hay tiempos insertados.";
    }
    #endregion
}
