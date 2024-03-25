using AgroGestor360.App.Views.Settings.Warehouse;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AgroGestor360.App.ViewModels.Settings;

public partial class CvWarehouseViewModel : ObservableObject
{
    [RelayCommand]
    async Task AddMerchandise()
    {
        await Shell.Current.GoToAsync(nameof(PgAddMerchandise), true);
    }

    [RelayCommand]
    async Task AddCategory()
    {
        string resul = await Shell.Current.DisplayPromptAsync("Agregar categoría", "Nombre:", "Agregar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(resul))
        {
            return;
        }
    }
}
