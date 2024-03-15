using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Settings.Products;

public partial class PgAddProductViewModel : ObservableValidator
{
    [ObservableProperty]
    string? salePrice;

    public string ArticleInfo
    {
        get
        {
            StringBuilder sb = new();
            sb.AppendLine($"CATEGORIA: {0}");
            sb.AppendLine($"PRECIO: {"0.00"}");
            sb.AppendLine($"PRESENTACION: {0}");
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
