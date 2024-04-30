using AgroGestor360.App.Models;
using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.App.Views.Settings.Warehouse;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class CvProductsViewModel : ObservableRecipient
{
    readonly IArticlesForSalesService articlesForSalesServ;
    readonly IProductsForSalesService productsForSalesServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvProductsViewModel(IArticlesForSalesService articlesForSalesService, IProductsForSalesService productsForSalesService, IAuthService authService)
    {
        articlesForSalesServ = articlesForSalesService;
        productsForSalesServ = productsForSalesService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        IsActive = true;
    }

    #region ARTICLE
    [ObservableProperty]
    ObservableCollection<DTO3>? articles;

    [ObservableProperty]
    DTO3? selectedArticle;

    [ObservableProperty]
    bool isZeroPrice;

    [RelayCommand]
    async Task GetArticlesZeroPrice()
    {
        var getArticles = await articlesForSalesServ.GetAllAsync(serverURL);
        if (getArticles is null)
        {
            return;
        }
        var articleszeroprice = getArticles!.Where(x => x.Price == 0);
        if (articleszeroprice.Any())
        {
            Articles = new(articleszeroprice);
            IsZeroPrice = true;
        }
        SelectedArticle = null;
    }

    [RelayCommand]
    async Task GetArticlesNonZeroPrice()
    {
        var getArticles = await articlesForSalesServ.GetAllAsync(serverURL);
        if (getArticles is null)
        {
            return;
        }
        var articleszeroprice = getArticles!.Where(x => x.Price > 0);
        if (articleszeroprice.Any())
        {
            Articles = new(articleszeroprice);
            IsZeroPrice = false;
        }
        SelectedArticle = null;
    }

    [RelayCommand]
    async Task ShowArticlePriceChange()
    {
        StringBuilder sb = new();
        sb.AppendLine("Va a cambiar el precio inicial del siguiente artículo:");
        sb.AppendLine($"Nombre: {SelectedArticle!.MerchandiseName}");
        if (SelectedArticle!.Packaging is not null)
        {
            sb.AppendLine($"Empaque: {SelectedArticle!.Packaging!.Value} {SelectedArticle!.Packaging!.Unit}");
        }
        sb.AppendLine($"Precio inicial actual: {SelectedArticle!.Price}");
        sb.AppendLine("Especifique la cantidad:");

        var newPrice = await Shell.Current.DisplayPromptAsync("Modificar precio inicial", sb.ToString(), "Modificar", "Cancelar", "0.00", 10, Keyboard.Numeric);

        if (string.IsNullOrEmpty(newPrice) || !double.TryParse(newPrice, out double thePrice))
        {
            SelectedArticle = null;
            return;
        }

        var result = await articlesForSalesServ.UpdateAsync(serverURL, new() { MerchandiseId = SelectedArticle!.MerchandiseId, Price = SelectedArticle!.Price + thePrice });

        if (result)
        {
            if (IsZeroPrice)
            {
                Articles!.Remove(SelectedArticle!);
                if (Articles!.Count == 0)
                {
                    await GetArticlesNonZeroPrice();
                }
            }
            else
            {
                int idx = Articles!.IndexOf(SelectedArticle);
                SelectedArticle.Price += thePrice;
                Articles[idx] = SelectedArticle;
            }
        }
    }
    #endregion

    #region PRODUCT
    [ObservableProperty]
    ObservableCollection<DTO4>? products;

    [ObservableProperty]
    DTO4? selectedProduct;

    [RelayCommand]
    async Task AddProduct()
    {
        Dictionary<string, object> sendObject = new() { { "CurrentArticle", SelectedArticle! } };
        await Shell.Current.GoToAsync(nameof(PgAddProduct), true, sendObject);
    }

    [RelayCommand]
    async Task DeleteProduct()
    {
        StringBuilder sb = new();
        sb.AppendLine($"Seguro que quiere eliminar el siguiente producto:");
        sb.AppendLine($"Nombre: {SelectedProduct!.ProductName}");
        sb.AppendLine($"Presentación: {SelectedProduct!.Packaging?.Value} {SelectedProduct!.Packaging?.Unit}");
        sb.AppendLine($"Precio: {SelectedProduct!.ArticlePrice.ToString("0.00")}");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar producto", sb.ToString().Trim(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedProduct = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedProduct = null;
            return;
        }

        var result = await productsForSalesServ.DeleteAsync(serverURL, SelectedProduct!.Id!);
        if (result)
        {
            Products!.Remove(SelectedProduct!);
        }
    }
    #endregion

    #region OFFER
    [ObservableProperty]
    ObservableCollection<ProductOffering>? offers;

    [ObservableProperty]
    ProductOffering? selectedOffert;

    [RelayCommand]
    async Task CreateOffer()
    {
        Dictionary<string, object> sendObject = new() {
            { "CurrentProduct", SelectedProduct! },
            { "OfferId", (Offers?.Max(x => x.Id) ?? 0) + 1 }
        };
        await Shell.Current.GoToAsync(nameof(PgCreateOffer), true, sendObject);
    }
    #endregion

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvProductsViewModel, PgAddProductMessage, string>(this, nameof(PgAddProductMessage), async (r, m) =>
        {
            DTO4_1 newProduct = new() { 
                ProductQuantity = m.Quantity, 
                MerchandiseId = SelectedArticle!.MerchandiseId,
                ProductName = m.Name,
                ArticlePrice = m.Quantity * SelectedArticle!.Price,
                Packaging = SelectedArticle!.Packaging
            };
            var result = await productsForSalesServ.InsertAsync(serverURL, newProduct);
            if (!string.IsNullOrEmpty(result))
            {
                r.Products ??= [];
                r.Products.Insert(0, new DTO4()
                {
                    Id = result,
                    MerchandiseId = SelectedArticle!.MerchandiseId,
                    ProductQuantity = m.Quantity,
                    ProductName = m.Name,
                    ArticlePrice = m.Quantity * SelectedArticle!.Price,
                    Packaging = SelectedArticle!.Packaging
                });
            }
            SelectedArticle = null;
        });

        WeakReferenceMessenger.Default.Register<CvProductsViewModel, ProductOffering, string>(this, "NewProductOffering", async (r, m) =>
        {
            bool result = await productsForSalesServ.UpdateAsync(serverURL, new DTO4_3() { Id = SelectedProduct!.Id, Offer = m });
            if (result)
            {
                r.Offers ??= [];
                r.Offers.Insert(0, m);
            }

        });

        WeakReferenceMessenger.Default.Register<CvProductsViewModel, string, string>(this, nameof(CvProductsViewModel), (r, m) =>
        {
            if (m == "cancel")
            {
                r.SelectedArticle = null;
                r.SelectedProduct = null;
                r.SelectedOffert = null;
                IsActive = false;
            }
        });
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectedProduct))
        {
            if (SelectedProduct is not null)
            {
                Offers ??= [];

                var getOffers = await productsForSalesServ.UpdateAsync(serverURL, SelectedProduct!.Id!);
                //if (getOffers?.Offering?.Any() ?? false)
                //{
                //    Offers = new(getOffers!.Offering);
                //}
                //else
                //{
                //    Offers = null;
                //}
            }
        }
    }

    #region EXTRA
    public async void Initialize()
    {
        await Task.WhenAll(GetArticles(), GetProducts());
    }

    async Task GetArticles()
    {
        await GetArticlesZeroPrice();
        if (Articles is null)
        {
            await GetArticlesNonZeroPrice();
        }
    }

    async Task GetProducts()
    {
        bool exist = await productsForSalesServ.CheckExistence(serverURL);
        if (exist)
        {
            var getProducts = await productsForSalesServ.GetAllAsync(serverURL);
            if (getProducts is null)
            {
                return;
            }
            Products = new(getProducts);
        }
    }
    #endregion
}
