using AgroGestor360.App.Models;
using AgroGestor360.App.Views.Settings.Products;
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
    readonly string serverURL;

    public CvProductsViewModel(IArticlesForSalesService articlesForSalesService, IProductsForSalesService productsForSalesService)
    {
        articlesForSalesServ = articlesForSalesService;
        productsForSalesServ = productsForSalesService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        IsActive = true;
    }

    #region ARTICLE
    [ObservableProperty]
    ObservableCollection<DTO3_1>? articles;

    [ObservableProperty]
    DTO3_1? selectedArticle;

    [ObservableProperty]
    bool isZeroPrice;

    [RelayCommand]
    async Task GetArticlesZeroPrice()
    {
        var getArticles = await articlesForSalesServ.GetAll1Async(serverURL);
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
    }

    [RelayCommand]
    async Task GetArticlesNonZeroPrice()
    {
        var getArticles = await articlesForSalesServ.GetAll1Async(serverURL);
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
    }

    [RelayCommand]
    async Task ShowSetSellingPrice()
    {
        var theSelection = SelectedArticle;
        SelectedArticle = null;

        StringBuilder sb = new();
        sb.AppendLine($"NOMBRE: {theSelection!.Name}");
        if (theSelection.Price > 0)
        {
            sb.AppendLine($"PRECIO ANTERIOR: {theSelection.Price.ToString("0.00")}");
        }
        sb.AppendLine($"PRESENTACION: {theSelection.Value.ToString("0.00")} {theSelection.Unit}");
        if (!string.IsNullOrEmpty(theSelection.Category))
        {
            sb.AppendLine($"CATEGORIA: {theSelection.Category}");
        }

        string price = await Shell.Current.DisplayPromptAsync("Establecer precio de venta", sb.ToString(), "Establecer", "Cancelar", "0.00");
        if (string.IsNullOrEmpty(price) || !double.TryParse(price, out double changePrice))
        {
            return;
        }
        var entity = new DTO3() { Id = theSelection!.Id, Price = changePrice };
        var result = await articlesForSalesServ.ChangePriceAsync(serverURL, entity);
        if (result)
        {
            if (IsZeroPrice)
            {
                Articles!.Remove(theSelection);
                if (!Articles.Any())
                {
                    await GetArticlesNonZeroPrice();
                }
            }
            else
            {
                int idx = Articles!.IndexOf(theSelection);
                theSelection.Price = changePrice;
                Articles[idx] = theSelection;
            }
        }
    }
    #endregion

    #region PRODUCT
    [ObservableProperty]
    ObservableCollection<DTO4_1>? products;

    [ObservableProperty]
    DTO4_1? selectedProduct;

    [RelayCommand]
    async Task AddProduct()
    {
        Dictionary<string, object> sendObject = new() { { "CurrentArticle", SelectedArticle! } };
        await Shell.Current.GoToAsync(nameof(PgAddProduct), true, sendObject);
    }

    [RelayCommand]
    async Task DeleteProduct()
    {
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
            var article = await articlesForSalesServ.GetByIdAsync(serverURL, m.ArticleId);
            DTO4 newProduct = new() { Name = m.Name, Article = article, Quantity = m.Quantity };
            var result = await productsForSalesServ.InsertAsync(serverURL, newProduct);
            if (!string.IsNullOrEmpty(result))
            {
                r.Products ??= [];
                r.Products.Insert(0, new DTO4_1()
                {
                    Quantity = m.Quantity,
                    Category = article!.Merchandise!.Category!.Name,
                    Name = m.Name,
                    Value = article!.Merchandise!.Packaging!.Value,
                    Unit = article!.Merchandise!.Packaging!.Unit,
                    SalePrice = m.Quantity * article!.Price
                });
            }
        });

        WeakReferenceMessenger.Default.Register<CvProductsViewModel, ProductOffering, string>(this, "NewProductOffering", async (r, m) =>
        {
            bool result = await productsForSalesServ.ChangeOfferingAsync(serverURL, new() { Id = SelectedProduct!.Id, Offering = [m] });
            if (result)
            {
                r.Offers ??= [];
                r.Offers.Insert(0, m);
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

                var getOffers = await productsForSalesServ.GetProductOfferingAsync(serverURL, SelectedProduct!.Id!);
                if (getOffers?.Offering?.Any() ?? false)
                {
                    Offers = new(getOffers!.Offering);
                }
                else
                {
                    Offers = null;
                }
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
        bool exist = await articlesForSalesServ.CheckExistence(serverURL);
        if (exist)
        {
            await GetArticlesZeroPrice();
            if (Articles is null)
            {
                await GetArticlesNonZeroPrice();
            }
        }
    }

    async Task GetProducts()
    {
        bool exist = await productsForSalesServ.CheckExistence(serverURL);
        if (exist)
        {
            var getProducts = await productsForSalesServ.GetAll1Async(serverURL);
            if (getProducts is null)
            {
                return;
            }
            Products = new(getProducts);
        }
    }
    #endregion
}
