using AgroGestor360.App.Models;
using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentArticle), nameof(CurrentArticle))]
public partial class PgAddProductViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArticleInfo))]
    DTO3_1? currentArticle;

    public string ArticleInfo
    {
        get
        {
            Name = CurrentArticle?.Name;
            StringBuilder sb = new();
            sb.AppendLine($"ARTICULO: {CurrentArticle?.Name}");
            if (!string.IsNullOrEmpty(CurrentArticle?.Category))
            {
                sb.AppendLine($"CATEGORIA: {CurrentArticle!.Category}");
            }
            sb.AppendLine($"PRECIO: {CurrentArticle?.Price.ToString("0.00")}");
            sb.AppendLine($"PRESENTACION: {CurrentArticle?.Value.ToString("0.00")} {CurrentArticle?.Unit}");
            return sb.ToString().TrimEnd();
        }
    }

    [ObservableProperty]
    [Required]
    [MinLength(3)]
    string? name;

    [ObservableProperty]
    [Required]
    string? quantity;

    [ObservableProperty]
    string? salePrice;

    [ObservableProperty]
    bool isVisibleInfo;

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();


        if (HasErrors || !double.TryParse(Quantity, out double theQuantity))
        {
            IsVisibleInfo = true;
            await Task.Delay(4000);
            IsVisibleInfo = false;
            return;
        }

        _ = WeakReferenceMessenger.Default.Send(new PgAddProductMessage(CurrentArticle!.Id!, Name!.Trim().ToUpper(), theQuantity), nameof(PgAddProductMessage));
        await Cancel();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Quantity))
        {
            SalePrice = double.TryParse(Quantity, out double theQuantity) ? (theQuantity * CurrentArticle?.Price ?? 0).ToString() : "0.00";
        }
    }
}
