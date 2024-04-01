using AgroGestor360.App.Views.Settings.Customers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial  class CvCustomersViewModel : ObservableObject
{
    [RelayCommand]
    async Task ShowAddCustomer()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditCustomer), true, new Dictionary<string, object>() { { "title", "Agregar" } });
    }

    [RelayCommand]
    async Task ShowEditCustomer()
    {
        await Shell.Current.GoToAsync(nameof(PgAddEditCustomer), true, new Dictionary<string, object>() { { "title", "Editar" } });
    }

    [RelayCommand]
    async Task ShowDeleteCustomer()
    {
        StringBuilder sb = new();
        sb.AppendLine("Usted esta seguro de eliminar al siguiente cliente:");
        sb.AppendLine($"FECHA DE INGRESO: {0}");
        sb.AppendLine($"NOMBRE: {0}");
        sb.AppendLine($"NIT: {0}");
        sb.AppendLine($"DPI: {0}");
        sb.AppendLine($"DIRECCION: {0}");
        sb.AppendLine($"TELEFONO: {0}");
        sb.AppendLine($"CORREO ELECTRONICO: {0}");

        bool resul = await Shell.Current.DisplayAlert("Eliminar cliente", sb.ToString().TrimEnd(), "Eliminar", "Cancelar");
        if (resul)
        {
            return;
        }
    }
}
