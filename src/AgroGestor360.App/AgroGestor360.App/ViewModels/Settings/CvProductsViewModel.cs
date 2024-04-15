using AgroGestor360.App.Views.Settings.Products;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Input;
using System.Collections.ObjectModel;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class CvProductsViewModel : ObservableRecipient
{
    readonly IArticlesForSalesService articlesForSalesServ;
    readonly string serverURL;

    public CvProductsViewModel(IArticlesForSalesService articlesForSalesService)
    {
        articlesForSalesServ = articlesForSalesService;
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
        double changePrice = 0;
        if (string.IsNullOrEmpty(price) || !double.TryParse(price, out changePrice))
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
    #endregion
}
