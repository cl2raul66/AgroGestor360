using AgroGestor360.App.Views.Settings.Shareholders;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Settings;

public partial class CvShareholdersViewModel : ObservableObject
{
    [RelayCommand]
    async Task ShowAddShareholder()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditShareholder), true, new Dictionary<string, object>() { { "title", "Agregar" } });
    }

    [RelayCommand]
    async Task ShowEditShareholder()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditShareholder), true, new Dictionary<string, object>() { { "title", "Editar" } });
    }

    [RelayCommand]
    async Task ShowDeleteShareholder()
    {
        StringBuilder sb = new();
        sb.AppendLine($"FECHA DE INGRESO: {0}");
        sb.AppendLine($"NOMBRE: {0}");
        sb.AppendLine($"NIT: {0}");
        sb.AppendLine($"DPI: {0}");
        sb.AppendLine($"DIRECCION: {0}");
        sb.AppendLine($"TELEFONO: {0}");
        sb.AppendLine($"CORREO ELECTRONICO: {0}");

        bool resul = await Shell.Current.DisplayAlert("Eliminar accionista", sb.ToString(), "Eliminar", "Cancelar");
        if (resul)
        {
            return;
        }
    }
}
