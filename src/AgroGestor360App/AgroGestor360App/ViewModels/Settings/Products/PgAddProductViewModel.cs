using AgroGestor360Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgroGestor360App.ViewModels;

[QueryProperty(nameof(SendToken), nameof(SendToken))]
[QueryProperty(nameof(CurrentArticle), nameof(CurrentArticle))]
public partial class PgAddProductViewModel : ObservableValidator
{
    [ObservableProperty]
    string? sendToken;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArticleInfo))]
    DTO3? currentArticle;

    public string ArticleInfo
    {
        get
        {
            StringBuilder sb = new();
            sb.AppendLine($"ARTICULO: {CurrentArticle?.MerchandiseName}");
            sb.AppendLine($"PRESENTACION: {CurrentArticle?.Packaging?.Value.ToString("0.00")} {CurrentArticle?.Packaging?.Unit}");
            sb.AppendLine($"PRECIO: {CurrentArticle?.Price.ToString("0.00")}");
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
    void CopyName()
    {
        Name = CurrentArticle?.MerchandiseName;
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", SendToken!);

        await Shell.Current.GoToAsync("..", true);
    }

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

        DTO4_1 newProduct = new()
        {
            ProductQuantity = theQuantity,
            MerchandiseId = CurrentArticle!.MerchandiseId,
            ArticlePrice = theQuantity * CurrentArticle!.Price,
            ProductName = Name!.Trim().ToUpper(),
            Packaging = CurrentArticle!.Packaging
        };

        _ = WeakReferenceMessenger.Default.Send(newProduct, "newProduct");

        await Shell.Current.GoToAsync("..", true);
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
