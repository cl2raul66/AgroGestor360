using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroGestor360.App.ViewModels;

public partial class CvProductsViewModel : ObservableRecipient
{
    readonly IArticlesService articlesServ;
    readonly string serverURL;

    public CvProductsViewModel(IArticlesService articlesService)
    {
        articlesServ = articlesService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        IsActive = true;
    }

    #region ARTICLE
    [ObservableProperty]
    ObservableCollection<Article>? articles;

    [ObservableProperty]
    Article? selectedArticle;

    [ObservableProperty]
    bool isZeroPrice;

    [RelayCommand]
    async Task GetArticlesZeroPrice()
    {
        var getArticles = await articlesServ.GetAllEnabledAsync(serverURL);
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
        var getArticles = await articlesServ.GetAllEnabledAsync(serverURL);
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
    #endregion

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

    #region EXTRA
    public async void Initialize()
    {
        //await Task.WhenAll(GetCategories(), GetWarehouse());
        await GetArticles();
    }

    async Task GetArticles()
    {
        bool exist = await articlesServ.CheckExistence(serverURL);
        if (exist)
        {
            await GetArticlesZeroPrice();
        }
    }
    #endregion
}
