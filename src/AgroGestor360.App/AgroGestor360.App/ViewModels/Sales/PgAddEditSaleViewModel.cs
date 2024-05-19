using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using AgroGestor360.App.Models;
using AgroGestor360.Client.Tools;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Sellers), "sellers")]
[QueryProperty(nameof(Customers), "customers")]
[QueryProperty(nameof(Products), "products")]
[QueryProperty(nameof(CurrentInvoice), "currentInvoice")]
public partial class PgAddEditSaleViewModel : ObservableValidator
{
    readonly IProductsForSalesService productsForSalesServ;
    readonly IArticlesForWarehouseService articlesForWarehouseServ;
    readonly IQuotesService quotesServ;
    readonly IImmediatePaymentTypeService immediatePaymentTypeServ;
    readonly ICreditPaymentTypeService creditPaymentTypeServ;
    readonly string serverURL;

    public PgAddEditSaleViewModel(IProductsForSalesService productsForSalesService, IArticlesForWarehouseService articlesForWarehouseService, IQuotesService quotesService, IImmediatePaymentTypeService immediatePaymentTypeService, ICreditPaymentTypeService creditPaymentTypeService)
    {
        productsForSalesServ = productsForSalesService;
        articlesForWarehouseServ = articlesForWarehouseService;
        quotesServ = quotesService;
        immediatePaymentTypeServ = immediatePaymentTypeService;
        creditPaymentTypeServ = creditPaymentTypeService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        Date = DateTime.Now;
        PaymentsTypes = new(immediatePaymentTypeServ.GetAll());
    }

    [ObservableProperty]
    DTO10? currentInvoice;

    [ObservableProperty]
    int productsPending;

    [ObservableProperty]
    string? textInfo;

    [ObservableProperty]
    DateTime date;

    [ObservableProperty]
    string? noFEL;

    [ObservableProperty]
    bool hasNoFEL;

    [ObservableProperty]
    double stock;

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
    ObservableCollection<string>? paymentsTypes;

    [ObservableProperty]
    string? selectedPaymentType;

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
    double total;

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
        if (Stock < theQuantity)
        {
            ProductsPending += 1;
        }

        ProductItems ??= [];
        ProductItems.Insert(0, new() { ProductItemQuantity = theQuantity, Product = SelectedProduct! });

        UpdateTotalQuote();

        SelectedProduct = null;
        Quantity = null;
    }

    [RelayCommand]
    async Task RemoveProductitem(ProductItem productitem)
    {
        var theQuantity = (await articlesForWarehouseServ.GetByIdAsync(serverURL, SelectedProductItem?.Product?.Id ?? string.Empty))?.Quantity ?? 0;

        var selectedQuantity = SelectedProductItem!.ProductOffer is null ? SelectedProductItem!.ProductItemQuantity : SelectedProductItem!.ProductOffer.Quantity;
        if (theQuantity < selectedQuantity)
        {
            ProductsPending -= 1;
        }
        bool result = ProductItems!.Remove(SelectedProductItem!);
        if (result)
        {
            UpdateTotalQuote();
            return;
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
                PriceWhitDiscount = SelectedProductItem!.Product!.ArticlePrice - (SelectedCustomer!.Discount!.Value / 100.00 * SelectedProductItem!.Product!.ArticlePrice),
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
        UpdateTotalQuote();
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

        DTO10_1 invoice = new()
        {
            Code = CurrentInvoice is not null ? CurrentInvoice.Code : string.Empty,
            NumberFEL = NoFEL,
            Status = InvoiceStatus.Pending,
            Date = Date,
            CustomerId = SelectedCustomer!.CustomerId,
            SellerId = SelectedSeller!.Id,
            Products = [.. productItems],
            CreditsPayments = OnCredit ? [new() { Type = creditPaymentTypeServ.GetByName(SelectedPaymentType!)! }] : null,
            ImmediatePayments = OnCredit ? null : [new() { Type = immediatePaymentTypeServ.GetByName(SelectedPaymentType!)! }]
        };

        string token = CurrentInvoice is not null ? "addorderfromquote" : "addinvoice";

        _ = WeakReferenceMessenger.Default.Send(invoice, token);

        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Cancel()
    {
        _ = WeakReferenceMessenger.Default.Send("cancel", nameof(PgSalesViewModel));
        await Shell.Current.GoToAsync("..");
    }

    //bool WaitPropertyChanged;

    protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(HasNoFEL))
        {
            NoFEL = HasNoFEL ? string.Empty : null;
        }

        if (e.PropertyName == nameof(OnCredit))
        {
            PaymentsTypes = OnCredit
                ? new(creditPaymentTypeServ.GetAll())
                : new(immediatePaymentTypeServ.GetAll());
        }

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
                //WaitPropertyChanged = false;
            }
        }

        if (e.PropertyName == nameof(SelectedProduct))
        {
            if (SelectedProduct is not null)
            {
                Stock = (await articlesForWarehouseServ.GetByIdAsync(serverURL, SelectedProduct.MerchandiseId!))?.Quantity ?? 0;
                //WaitPropertyChanged = false;
            }
        }

        //if (e.PropertyName == nameof(CurrentInvoice))
        //{
        //    if (CurrentInvoice is not null)
        //    {
        //        SelectedSeller = Sellers?.FirstOrDefault(x => x.Id == CurrentInvoice.Seller!.Id);
        //        SelectedCustomer = Customers?.FirstOrDefault(x => x.CustomerId == CurrentInvoice.Customer!.CustomerId);
        //        foreach (var item in CurrentInvoice.Products!)
        //        {
        //            Quantity = item.Quantity.ToString("F2");
        //            SelectedProduct = Products?.FirstOrDefault(x => x.Id == item!.ProductItemForSaleId);
        //            WaitPropertyChanged = true;
        //            while (WaitPropertyChanged)
        //            {
        //                await Task.Delay(1000);
        //            }
        //            await SendProductItem();
        //            SelectedProductItem = ProductItems?.FirstOrDefault(x => x.Product!.Id == item.ProductItemForSaleId);
        //            WaitPropertyChanged = true;
        //            while (WaitPropertyChanged)
        //            {
        //                await Task.Delay(1000);
        //            }
        //            if (item.HasCustomerDiscount)
        //            {
        //                IsCustomerDiscount = true;
        //            }
        //            else if (item.OfferId > 0)
        //            {
        //                IsProductOffer = true;
        //                SelectedOffer = Offers?.FirstOrDefault(x => x.Id == item.OfferId);
        //            }
        //            else
        //            {
        //                IsNormalPrice = true;
        //            }
        //            SetDiscount();
        //        }
        //    }
        //}
    }

    #region EXTRA
    // todo: no se puede modificar vendedor ni cliente una vez introducido el primer producto, se debe limpiar la lista de productos para poder modificar. recordar que se debe validar que el producto no se repita en la lista y que la cantidad sea mayor a 0, ademas de agregar botones para modificar tanto el cliente como el vendedor

    private void UpdateTotalQuote()
    {
        double total = 0;
        if (ProductItems != null)
        {
            foreach (var item in ProductItems)
            {
                double itemPrice = item.Product!.ArticlePrice;
                if (item.CustomerDiscountClass is not null)
                {
                    double discount = item.CustomerDiscountClass.Value;
                    itemPrice -= itemPrice * (discount / 100);
                }

                double itemQuantity = item.ProductItemQuantity;
                if (item.ProductOffer is not null)
                {
                    itemQuantity = item.ProductOffer!.Quantity;
                }
                total += itemQuantity * itemPrice;
            }
        }
        Total = total;
    }
    #endregion
}
