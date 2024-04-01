using AgroGestor360.App.Views.Settings.Products;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class CvProductsViewModel : ObservableObject
{
    //[ObservableProperty]
    //bool isProductsVisible;

    //[RelayCommand]
    //void ViewArticles()
    //{
    //    IsProductsVisible = false;
    //}

    //[RelayCommand]
    //void ViewProducts()
    //{
    //    IsProductsVisible = true;
    //}

    [RelayCommand]
    async Task ShowSetSellingPrice()
    {
        StringBuilder sb = new();
        sb.AppendLine($"NOMBRE: {0}");
        sb.AppendLine($"PRECIO ANTERIOR: {"0.00"}");
        sb.AppendLine($"PRESENTACION: {0}");
        sb.AppendLine($"CATEGORIA: {0}");

        string resul = await Shell.Current.DisplayPromptAsync("Establecer precio de venta", sb.ToString().TrimEnd(), "Establecer", "Cancelar", "0.00");
        if (string.IsNullOrEmpty(resul))
        {
            return;
        }
    }

    [RelayCommand]
    async Task AddProduct()
    {
        await Shell.Current.GoToAsync(nameof(PgAddProduct), true);
    }

    [RelayCommand]
    async Task CreateOffer()
    {
        await Shell.Current.GoToAsync(nameof(PgCreateOffer), true);
    }
}
