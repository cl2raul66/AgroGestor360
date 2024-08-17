using AgroGestor360.App.Models;
using AgroGestor360.App.Tools.Messages;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using AgroGestor360.Client.Tools;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Sellers), "sellers")]
[QueryProperty(nameof(Customers), "customers")]
[QueryProperty(nameof(Products), "products")]
public partial class PgAddEditQuoteViewModel : ObservableValidator
{
    readonly IProductsForSalesService productsForSalesServ;
    readonly IArticlesForWarehouseService articlesForWarehouseServ;
    readonly string serverURL;

    public PgAddEditQuoteViewModel(IProductsForSalesService productsForSalesService, IArticlesForWarehouseService articlesForWarehouseService)
    {
        productsForSalesServ = productsForSalesService;
        articlesForWarehouseServ = articlesForWarehouseService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        Date = DateTime.Now;
    }

    [ObservableProperty]
    int productsPending;

    [ObservableProperty]
    string? textInfo;

    [ObservableProperty]
    DateTime date;

    [ObservableProperty]
    List<DTO6>? sellers;

    [ObservableProperty]
    [Required]
    DTO6? selectedSeller;

    [ObservableProperty]
    List<DTO5_1>? customers;

    [ObservableProperty]
    [Required]
    DTO5_1? selectedCustomer;

    [ObservableProperty]
    List<DTO4>? products;

    [ObservableProperty]
    DTO4? selectedProduct;

    [ObservableProperty]
    string? quantity;

    [ObservableProperty]
    [Required]
    [MinLength(1)]
    ObservableCollection<ProductItem>? productItems;

    [ObservableProperty]
    ProductItem? selectedProductItem;

    [ObservableProperty]
    bool isNormalPrice;

    [ObservableProperty]
    bool isCustomerDiscount;

    [ObservableProperty]
    bool isProductOffer;

    [ObservableProperty]
    ObservableCollection<ProductOffering>? offers;

    [ObservableProperty]
    ProductOffering? selectedOffer;

    [ObservableProperty]
    double totalWithDiscount;

    [ObservableProperty]
    double total;

    [ObservableProperty]
    double difference;

    [RelayCommand]
    async Task SendProductItem()
    {
        if (SelectedCustomer is null)
        {
            TextInfo = " Debe seleccionar un cliente";
            await Task.Delay(4000);
            TextInfo = null;
            return;
        }
        if (!double.TryParse(Quantity, out double theQuantity))
        {
            TextInfo = " La cantidad debe ser un número";
            await Task.Delay(4000);
            TextInfo = null;
            return;
        }
        ProductItems ??= [];
        ProductItems.Insert(0, new() { ProductItemQuantity = theQuantity, Product = SelectedProduct! });

        UpdateTotal();

        SelectedProduct = null;
        Quantity = null;
    }

    [RelayCommand]
    async Task RemoveProductitem(ProductItem productitem)
    {
        string currentProductItem = SelectedProductItem!.Product!.MerchandiseId!;

        var theQuantity = (await articlesForWarehouseServ.GetByIdAsync(serverURL, SelectedProductItem?.Product?.Id ?? string.Empty))?.Quantity ?? 0;

        var selectedQuantity = SelectedProductItem!.ProductOffer is null ? SelectedProductItem!.ProductItemQuantity : SelectedProductItem!.ProductOffer.Quantity;
        if (theQuantity < selectedQuantity)
        {
            ProductsPending -= 1;
        }
        bool result = ProductItems!.Remove(SelectedProductItem!);
        if (result)
        {
            UpdateTotal();
            Quantity = null;
            SelectedProduct = null;
            SelectedProductItem = null;
            //return;
        }
        ProductsPending += 1;
    }

    [RelayCommand]
    void SetDiscount()
    {
        int idx = ProductItems!.IndexOf(SelectedProductItem!);
        ProductItem item = new();
        if (IsNormalPrice)
        {
            item = new()
            {
                ProductItemQuantity = SelectedProductItem!.ProductItemQuantity,
                PriceWhitDiscount = 0,
                Product = SelectedProductItem!.Product,
                ProductOffer = null,
                CustomerDiscountClass = null
            };
        }

        if (IsCustomerDiscount)
        {
            item = new()
            {
                ProductItemQuantity = SelectedProductItem!.ProductItemQuantity,
                PriceWhitDiscount = SelectedProductItem!.Product!.ArticlePrice - (SelectedCustomer!.Discount!.Discount / 100.00 * SelectedProductItem!.Product!.ArticlePrice),
                Product = SelectedProductItem!.Product,
                ProductOffer = null,
                CustomerDiscountClass = SelectedCustomer!.Discount
            };
        }

        if (IsProductOffer)
        {
            item = new()
            {
                ProductItemQuantity = SelectedProductItem!.ProductItemQuantity,
                PriceWhitDiscount = SelectedProductItem!.Product!.ArticlePrice * SelectedOffer!.Quantity,
                Product = SelectedProductItem!.Product,
                ProductOffer = SelectedOffer,
                CustomerDiscountClass = null
            };
        }
        ProductItems[idx] = item;
        SelectedProductItem = ProductItems[idx];
        UpdateTotal();
    }

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            TextInfo = " Rellene toda la información los requeridos (*)";
            await Task.Delay(4000);
            TextInfo = null;
            return;
        }

        List<DTO9> productItems = [];
        foreach (var x in ProductItems!)
        {
            if (x.CustomerDiscountClass is null && x.ProductOffer is null)
            {
                DTO9 dto = new()
                {
                    Quantity = x.ProductItemQuantity,
                    ProductItemForSaleId = x.Product!.Id
                };
                productItems.Add(dto);
            }

            if (x.CustomerDiscountClass is not null)
            {
                DTO9 dto = new()
                {
                    Quantity = x.ProductItemQuantity,
                    ProductItemForSaleId = x.Product!.Id,
                    HasCustomerDiscount = true
                };
                productItems.Add(dto);
            }

            if (x.ProductOffer is not null)
            {
                DTO9 dto = new()
                {
                    ProductItemForSaleId = x.Product!.Id,
                    OfferId = x.ProductOffer!.Id 
                };
                productItems.Add(dto);
            }
        }

        DTO7_1 quotation = new()
        {
            Status = QuotationStatus.Draft,
            Date = Date,
            CustomerId = SelectedCustomer!.CustomerId,
            SellerId = SelectedSeller!.Id,
            ProductItems = [.. productItems]
        };

        _ = WeakReferenceMessenger.Default.Send(quotation, "addquote");

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send(new CancelDialogForPgSalesRequestMessage(true));
        await Shell.Current.GoToAsync("..");
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(IsProductOffer))
        {
            if (!IsProductOffer)
            {
                Offers = null;
                SelectedOffer = null;
                return;
            }

            Offers = new(await productsForSalesServ.GetOffersById(serverURL, SelectedProductItem!.Product!.Id!));
            if (SelectedProductItem!.ProductOffer is null)
            {
                SelectedOffer = Offers![0];
                return;
            }

            var offer = Offers!.FirstOrDefault(x => x.Id == SelectedProductItem!.ProductOffer!.Id);
            int idx = Offers!.IndexOf(offer!);
            SelectedOffer = Offers[idx];
        }

        if (e.PropertyName == nameof(SelectedProductItem))
        {
            if (SelectedProductItem is not null)
            {
                if (SelectedProductItem.CustomerDiscountClass is not null)
                {
                    IsCustomerDiscount = true;
                }

                if (SelectedProductItem.ProductOffer is not null)
                {
                    IsProductOffer = true;
                }

                if (SelectedProductItem.CustomerDiscountClass is null && SelectedProductItem.ProductOffer is null)
                {
                    IsNormalPrice = true;
                }
            }
        }
    }

    #region EXTRA
    private void UpdateTotal()
    {
        double total = 0;
        double total1 = 0;
        if (ProductItems != null)
        {
            foreach (var item in ProductItems)
            {
                double itemPrice = item.Product!.ArticlePrice;
                if (item.CustomerDiscountClass is not null)
                {
                    double discount = item.CustomerDiscountClass.Discount;
                    itemPrice -= itemPrice * (discount / 100);
                }

                double itemQuantity = item.ProductItemQuantity;
                if (item.ProductOffer is not null)
                {
                    itemQuantity = item.ProductOffer!.Quantity;
                }
                total1 += itemQuantity * itemPrice;
                total += itemQuantity * item.Product!.ArticlePrice;
            }
        }
        Total = total;
        TotalWithDiscount = total1;
        Difference = total - total1;
    }
    #endregion
}
