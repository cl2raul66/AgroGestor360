﻿using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentProduct), nameof(CurrentProduct))]
[QueryProperty(nameof(OfferId), nameof(OfferId))]
public partial class PgCreateOfferViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NameOffer))]
    [NotifyPropertyChangedFor(nameof(ProductInfo))]
    DTO4_1? currentProduct;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NameOffer))]
    int offerId;

    [ObservableProperty]
    [Required]
    string? unitQuantity;

    [ObservableProperty]
    [Required]
    string? amountToBeRebated;

    [ObservableProperty]
    string? salePrice;

    public string NameOffer => $"{CurrentProduct?.Name} - {OfferId}";

    public string ProductInfo
    {
        get
        {
            StringBuilder sb = new();
            sb.AppendLine($"Nombre: {CurrentProduct?.Name}");
            sb.AppendLine($"CATEGORIA: {CurrentProduct?.Category}");
            sb.AppendLine($"PRECIO: {CurrentProduct?.SalePrice.ToString("0.00")}");
            sb.AppendLine($"PRESENTACION: {CurrentProduct?.Value} {CurrentProduct?.Unit}");
            return sb.ToString().TrimEnd();
        }
    }

    [ObservableProperty]
    bool isVisibleInfo;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors || !double.TryParse(UnitQuantity, out var quantity) || !double.TryParse(AmountToBeRebated, out double rebated))
        {
            IsVisibleInfo = true;
            await Task.Delay(4000);
            IsVisibleInfo = false;
            return;
        }

        ProductOffering newProductOffering = new() { Id = OfferId, Quantity = quantity, BonusAmount = rebated };

        _ = WeakReferenceMessenger.Default.Send(newProductOffering, "NewProductOffering");

        await Cancel();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(UnitQuantity))
        {
            SalePrice = string.IsNullOrEmpty(UnitQuantity) || !double.TryParse(UnitQuantity, out var quantity)
                ? "0.00"
                : (quantity * CurrentProduct!.SalePrice).ToString("0.00");
        }
    }
}
