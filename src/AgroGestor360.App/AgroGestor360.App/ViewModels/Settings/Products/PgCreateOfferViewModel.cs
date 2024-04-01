﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class PgCreateOfferViewModel : ObservableValidator
{
    [ObservableProperty]
    string? name;

    [ObservableProperty]
    string? salePrice;

    public string ProductInfo
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
