using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360.App.ViewModels;

public partial class CvSeedCapitalViewModel : ObservableObject
{
    [RelayCommand]
    async Task Add()
    {
        string resul = await Shell.Current.DisplayPromptAsync("Agregar capital inicial", "", "Agregar", "Cancelar", "0.00");
        if (string.IsNullOrEmpty(resul))
        {
            return;
        }

    }
}
