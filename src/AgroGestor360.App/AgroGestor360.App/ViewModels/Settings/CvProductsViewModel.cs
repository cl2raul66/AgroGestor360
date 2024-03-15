using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels.Settings;

public partial class CvProductsViewModel : ObservableObject
{
    [ObservableProperty]
    bool isProductsVisible;

    [RelayCommand]
    void ViewArticles()
    {
        IsProductsVisible = false;
    }

    [RelayCommand]
    void ViewProducts()
    {
        IsProductsVisible = true;
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
