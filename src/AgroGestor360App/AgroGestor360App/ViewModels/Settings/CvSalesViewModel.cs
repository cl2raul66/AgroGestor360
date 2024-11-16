using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using AgroGestor360Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360App.ViewModels;

public partial class CvSalesViewModel : ObservableValidator
{
    readonly IReconciliationService reconciliationServ;
    readonly ITypeFrequencyReconciliationPolicyService typeFrequencyReconciliationPolicyServ;
    readonly string serverURL;

    public CvSalesViewModel(IReconciliationService reconciliationService, ITypeFrequencyReconciliationPolicyService typeFrequencyReconciliationPolicyService)
    {
        reconciliationServ = reconciliationService;
        typeFrequencyReconciliationPolicyServ = typeFrequencyReconciliationPolicyService;
        FrequencyList = [.. typeFrequencyReconciliationPolicyServ.GetAll()];
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
    }

    [ObservableProperty]
    List<string>? frequencyList;

    [ObservableProperty]
    [Required]
    string? selectedFrecuency;

    [ObservableProperty]
    [Required]
    TimeSpan time = new(0, 0, 0);

    [ObservableProperty]
    string? textInfo;

    [RelayCommand]
    async Task Save()
    {
        if (HasErrors || Time.Hours == 0)
        {
            TextInfo = "Complete los requeridos (*)";
            await Task.Delay(4000);
            TextInfo = null;
            return;
        }

        if (Time < new TimeSpan(15, 0, 0))
        {
            TextInfo = "Horario mayor a 03:00 PM.";
            await Task.Delay(4000);
            TextInfo = null;
            return;
        }

        var hasPolicy = await reconciliationServ.HasPolicyAsync(serverURL);
        var theFrequency = typeFrequencyReconciliationPolicyServ.GetByName(SelectedFrecuency!);

        if (theFrequency is null)
        {
            TextInfo = "Frecuencia no válida.";
            await Task.Delay(4000);
            TextInfo = null;
            return;
        }

        var result = await reconciliationServ.InsertPolicyAsync(serverURL, new ReconciliationPolicy() { Frequency = (TypeFrequencyReconciliationPolicy)theFrequency, Time = Time });

        TextInfo = result > 0 ? "Se guardo con éxito." : "No se pudo guardar.";
        await Task.Delay(4000);
        TextInfo = null;
    }
}
