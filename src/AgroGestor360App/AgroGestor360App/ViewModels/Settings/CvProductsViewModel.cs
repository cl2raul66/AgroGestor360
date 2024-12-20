﻿// Ignore Spelling: auth

using AgroGestor360App.Views.Settings;
using AgroGestor360Client.Models;
using AgroGestor360Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace AgroGestor360App.ViewModels;

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
    }

    [ObservableProperty]
    bool isBusy;

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
        AllSelectedAsNull();
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
        AllSelectedAsNull();
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
            AllSelectedAsNull();
            return;
        }

        var result = await articlesForSalesServ.UpdateAsync(serverURL, new() { MerchandiseId = SelectedArticle!.MerchandiseId, Price = thePrice });

        if (result)
        {
            string merchandiseId = SelectedArticle.MerchandiseId!;
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
                // Bloque A
                int idx = Articles!.IndexOf(SelectedArticle);
                SelectedArticle.Price = thePrice;
                Articles[idx] = SelectedArticle;

                // Bloque B
                await Task.Run(async () =>
                {
                    var foundProducts = Products!.Where(x => x.MerchandiseId == merchandiseId);
                    if (foundProducts.Any())
                    {
                        Products = new(await productsForSalesServ.GetAllAsync(serverURL));
                    }
                });
            }
        }
        AllSelectedAsNull();
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
        IsActive = true;
        Dictionary<string, object> sendData = new()
        {
            { "SendToken", "D3B07384-D9A7-4B9F-8F1E-2B0B0D5E6F7A" },
            { "CurrentArticle", SelectedArticle! }
        };
        await Shell.Current.GoToAsync(nameof(PgAddProduct), true, sendData);
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
        IsActive = true;
        Dictionary<string, object> sendData = new() {
            { "SendToken", "D3B07384-D9A7-4B9F-8F1E-2B0B0D5E6F7A" },
            { "CurrentProduct", SelectedProduct! },
            { "OfferId", Offers is not null && Offers.Any() ? Offers.Max(x => x.Id) + 1 : 1}
        };
        await Shell.Current.GoToAsync(nameof(PgCreateOffer), true, sendData);
    }

    [RelayCommand]
    async Task DeleteOffer()
    {
        DTO4_4 deletedOffer = new() { Id = SelectedProduct!.Id, OfferId = SelectedOffert!.Id! };

        var result = await productsForSalesServ.UpdateAsync(serverURL, deletedOffer);
        if (result)
        {
            Offers!.Remove(SelectedOffert!);
        }
    }
    #endregion

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvProductsViewModel, DTO4_1, string>(this, "newProduct", async (r, m) =>
        {
            var result = await productsForSalesServ.InsertAsync(serverURL, m);
            if (!string.IsNullOrEmpty(result))
            {
                r.Products ??= [];
                r.Products.Insert(0, new DTO4()
                {
                    Id = result,
                    MerchandiseId = m.MerchandiseId,
                    ProductQuantity = m.ProductQuantity,
                    ProductName = m.ProductName,
                    ArticlePrice = m.ArticlePrice,
                    Packaging = m.Packaging
                });
            }

            IsActive = false;
            AllSelectedAsNull();
        });

        WeakReferenceMessenger.Default.Register<CvProductsViewModel, DTO4_3, string>(this, "NewProductOffering", async (r, m) =>
        {
            int idx = r.Products!.IndexOf(SelectedProduct!);
            bool result = await productsForSalesServ.UpdateAsync(serverURL, m);
            if (result)
            {
                var currentProduct = r.Products![idx];
                currentProduct.HasOffers = true;
                r.Products![idx] = currentProduct!;
            }

            IsActive = false;
            AllSelectedAsNull();
            r.SelectedProduct = r.Products![idx];
        });

        WeakReferenceMessenger.Default.Register<CvProductsViewModel, string, string>(this, "D3B07384-D9A7-4B9F-8F1E-2B0B0D5E6F7A", (r, m) =>
        {
            if (m == "cancel")
            {
                AllSelectedAsNull();
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
                Offers = new(await productsForSalesServ.GetOffersById(serverURL, SelectedProduct.Id!));
            }
        }
    }
    public async void Initialize()
    {
        IsBusy = true;
        await Task.WhenAll(GetArticles(), GetProducts());
        IsBusy = false;
    }

    #region EXTRA
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

    void AllSelectedAsNull()
    {
        SelectedArticle = null;
        SelectedProduct = null;
        SelectedOffert = null;
    }
    #endregion
}
