using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Loans;

public partial class PgAmortizationViewModel : ObservableValidator
{
    public string ProductInfo
    {
        get
        {
            StringBuilder sb = new();
            sb.AppendLine($"No. PRESTAMO: {0}");
            sb.AppendLine($"TIPO: {"0.00"}");
            sb.AppendLine($"FECHA: {0}");
            sb.AppendLine($"CONCEPTO: {0}");
            sb.AppendLine($"DETALLES DEL PRESTAMO: {0}");
            sb.AppendLine($"MONTO INICIAL: {0}");
            sb.AppendLine($"MONTO PAGADO: {0}");
            return sb.ToString().TrimEnd();
        }
    }

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        await Cancel();
    }
}
