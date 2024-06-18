using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using AgroGestor360.App.Models;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Sellers), "sellers")]
[QueryProperty(nameof(Customers), "customers")]
[QueryProperty(nameof(Products), "products")]
[QueryProperty(nameof(CreditTime), "creditTime")]
[QueryProperty(nameof(DefaultCreditTime), "defaultcredittime")]
[QueryProperty(nameof(CurrentInvoice), "currentInvoice")]
public partial class PgAddEditSaleViewModel : ObservableValidator
{
    readonly IProductsForSalesService productsForSalesServ;
    readonly IArticlesForWarehouseService articlesForWarehouseServ;
    readonly IQuotesService quotesServ;
    readonly string serverURL;
    Dictionary<string, double>? StockInWarehouse;
    readonly SemaphoreSlim semaphore = new(0, 4);

    public PgAddEditSaleViewModel(IProductsForSalesService productsForSalesService, IArticlesForWarehouseService articlesForWarehouseService, IQuotesService quotesService)
    {
        productsForSalesServ = productsForSalesService;
        articlesForWarehouseServ = articlesForWarehouseService;
        quotesServ = quotesService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        Date = DateTime.Now;
    }

    [ObservableProperty]
    DTO_SB1? currentInvoice;

    [ObservableProperty]
    int productsPending;

    [ObservableProperty]
    string? textInfo;

    [ObservableProperty]
    TimeLimitForCredit[]? creditTime;

    [ObservableProperty]
    TimeLimitForCredit? selectedCreditTime;

    [ObservableProperty]
    TimeLimitForCredit? defaultCreditTime;

    [ObservableProperty]
    DateTime date;

    [ObservableProperty]
    double stock;

    [ObservableProperty]
    DTO6[]? sellers;

    [ObservableProperty]
    [Required]
    DTO6? selectedSeller;

    [ObservableProperty]
    DTO5_1[]? customers;

    [ObservableProperty]
    [Required]
    DTO5_1? selectedCustomer;

    [ObservableProperty]
    DTO4[]? products;

    [ObservableProperty]
    DTO4? selectedProduct;

    [ObservableProperty]
    string? quantity;

    [ObservableProperty]
    bool onCredit;

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

    [ObservableProperty]
    bool loadingStock;

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
        if (theQuantity <= 0)
        {
            TextInfo = " La cantidad debe ser mayor a 0";
            await Task.Delay(4000);
            TextInfo = null;
            return;
        }        

        ProductItems ??= [];
        ProductItems.Insert(0, new() { ProductItemQuantity = theQuantity, Product = SelectedProduct! });

        UpdateTotal();
        await UpdateStock(SelectedProduct!.MerchandiseId!, theQuantity);
        
        if (Stock < theQuantity)
        {
            ProductsPending += 1;
        }

        SelectedProduct = null;
        Quantity = null;
        Stock = 0;
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
            StockInWarehouse!.Remove(currentProductItem);
            Quantity = null;
            SelectedProduct = null;
            SelectedProductItem = null;
            Stock = 0;
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

    /// <summary>
    /// Método que se ejecuta al presionar el botón "Add" para agregar una venta.
    /// Realiza las validaciones necesarias y crea una nueva venta con los productos seleccionados.
    /// </summary>
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
                DTO9 dTO9 = new()
                {
                    Quantity = x.ProductItemQuantity,
                    ProductItemForSaleId = x.Product!.Id
                };
                productItems.Add(dTO9);
            }

            if (x.CustomerDiscountClass is not null)
            {
                DTO9 dTO9 = new()
                {
                    Quantity = x.ProductItemQuantity,
                    ProductItemForSaleId = x.Product!.Id,
                    HasCustomerDiscount = true
                };
                productItems.Add(dTO9);
            }

            if (x.ProductOffer is not null)
            {
                DTO9 dTO9 = new()
                {
                    ProductItemForSaleId = x.Product!.Id,
                    OfferId = x.ProductOffer!.Id
                };
                productItems.Add(dTO9);
            }
        }

        DTO10_1 dTO = new()
        {
            Code = CurrentInvoice is null ? string.Empty : CurrentInvoice.Code,
            Date = Date,
            TimeCredit = SelectedCreditTime,
            CustomerId = SelectedCustomer!.CustomerId,
            SellerId = SelectedSeller!.Id,
            Status = Client.Tools.InvoiceStatus.Pending,
            Products = [.. productItems]
        };

        string token = CurrentInvoice is null ? "addinvoice" : "addinvoicefromorder";

        _ = WeakReferenceMessenger.Default.Send(dTO, token);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", nameof(PgSalesViewModel));
        await Shell.Current.GoToAsync("..");
    }

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(OnCredit))
        {
            SelectedCreditTime = OnCredit ? CreditTime!.First(x => x.Id == DefaultCreditTime!.Id) : null;
        }

        if (e.PropertyName == nameof(IsProductOffer))
        {
            if (!IsProductOffer)
            {
                Offers = null;
                SelectedOffer = null; 
                semaphore.Release();
                return;
            }

            Offers = new(await productsForSalesServ.GetOffersById(serverURL, SelectedProductItem!.Product!.Id!));
            if (SelectedProductItem!.ProductOffer is null)
            {
                SelectedOffer = Offers![0];
                semaphore.Release();
                return;
            }

            var offer = Offers!.FirstOrDefault(x => x.Id == SelectedProductItem!.ProductOffer!.Id);
            int idx = Offers!.IndexOf(offer!);
            SelectedOffer = Offers[idx];
            semaphore.Release();
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

        if (e.PropertyName == nameof(SelectedProduct))
        {
            if (SelectedProduct is not null)
            {
                LoadingStock = true;
                await UpdateStock(SelectedProduct!.MerchandiseId!);
                LoadingStock = false;
            }
        }

        if (e.PropertyName == nameof(Sellers))
        {
            semaphore.Release();
        }

        if (e.PropertyName == nameof(Customers))
        {
            semaphore.Release();
        }

        if (e.PropertyName == nameof(Products))
        {
            semaphore.Release();
        }

        if (e.PropertyName == nameof(CurrentInvoice))
        {
            if (CurrentInvoice is not null)
            {
                semaphore.Wait();
                semaphore.Wait();
                semaphore.Wait();
                SelectedSeller = Sellers?.FirstOrDefault(x => x.Id == CurrentInvoice.Seller!.Id);
                SelectedCustomer = Customers?.FirstOrDefault(x => x.CustomerId == CurrentInvoice.Customer!.CustomerId);
                foreach (var item in CurrentInvoice.Products!)
                {
                    Quantity = (item.Quantity == 0 ? 1 : item.Quantity).ToString("F2");
                    SelectedProduct = Products?.FirstOrDefault(x => x.Id == item!.ProductItemForSaleId);
                    await SendProductItem();
                    SelectedProductItem = ProductItems?.FirstOrDefault(x => x.Product!.Id == item.ProductItemForSaleId);
                    if (item.HasCustomerDiscount)
                    {
                        IsCustomerDiscount = true;
                    }
                    else if (item.OfferId > 0)
                    {
                        IsProductOffer = true;
                        await semaphore.WaitAsync();
                        SelectedOffer = Offers?.FirstOrDefault(x => x.Id == item.OfferId);
                    }
                    else
                    {
                        IsNormalPrice = true;
                    }
                    SetDiscount();
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

    /// <summary>
    /// Actualiza el stock disponible de un producto específico. Este método ajusta el stock en la memoria local
    /// y opcionalmente en un servicio o base de datos externa, asegurando que el inventario refleje correctamente
    /// las operaciones de venta o devolución.
    /// </summary>
    /// <param name="merchandiseId">El identificador único del producto cuyo stock se va a actualizar.</param>
    /// <param name="theQuantity">La cantidad por la cual se debe ajustar el stock. Un valor positivo indica una reducción
    /// del stock (venta), mientras que un valor negativo indica un incremento (devolución). El valor predeterminado es 0.</param>
    /// <returns>Una tarea que representa la operación asincrónica de actualización del stock.</returns>
    /// <remarks>
    /// Este método es asincrónico y debe ser esperado usando 'await'. Es importante manejar posibles errores durante
    /// las llamadas asincrónicas, especialmente cuando se interactúa con servicios externos o bases de datos.
    /// La actualización de la propiedad 'Stock' asegura que cualquier elemento de la UI vinculado a esta propiedad
    /// se actualice automáticamente para reflejar los cambios.
    /// </remarks>
    async Task UpdateStock(string merchandiseId, double theQuantity = 0)
    {
        StockInWarehouse ??= [];
        double value;
        if (StockInWarehouse.Count != 0)
        {
            if (StockInWarehouse.TryGetValue(SelectedProduct!.MerchandiseId!, out double theValue))
            {
                value = theValue;
            }
            else
            {
                value = (await articlesForWarehouseServ.GetByIdAsync(serverURL, SelectedProduct!.MerchandiseId!))?.Quantity ?? 0;
            }
        }
        else if (Stock == 0)
        {
            value = (await articlesForWarehouseServ.GetByIdAsync(serverURL, SelectedProduct!.MerchandiseId!))?.Quantity ?? 0;
        }
        else
        {
            value = Stock;
        }

        if (theQuantity > 0)
        {
            value -= theQuantity;
            StockInWarehouse.Add(SelectedProduct!.MerchandiseId!, value);
        }
        if (theQuantity < 0)
        {
            value += theQuantity;
            StockInWarehouse.Remove(SelectedProduct!.MerchandiseId!);
        }

        Stock = value;
    }
    #endregion
}
