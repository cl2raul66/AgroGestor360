using AgroGestor360.App.Models;
using AgroGestor360.App.Views;
using AgroGestor360.App.Views.Settings.Warehouse;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Text;

namespace AgroGestor360.App.ViewModels;

public partial class CvWarehouseViewModel : ObservableRecipient
{
    readonly IMerchandiseService merchandiseServ;
    readonly IArticlesForWarehouseService articlesForWarehouseServ;
    readonly IAuthService authServ;
    readonly IProductsForSalesService productsServ;
    readonly IArticlesForSalesService articlesForSalesServ;
    readonly string serverURL;

    public CvWarehouseViewModel(IArticlesForWarehouseService articlesForWarehouseService, IMerchandiseService merchandiseService, IAuthService authService, IProductsForSalesService productsForSalesService, IArticlesForSalesService articlesForSalesService)
    {
        merchandiseServ = merchandiseService;
        articlesForWarehouseServ = articlesForWarehouseService;
        authServ = authService;
        productsServ = productsForSalesService;
        articlesForSalesServ = articlesForSalesService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
    }

    [ObservableProperty]
    bool isBusy;

    [ObservableProperty]
    ObservableCollection<DTO2>? warehouses;

    [ObservableProperty]
    DTO2? selectedWarehouse;

    [RelayCommand]
    async Task SetMerchandiseEntry()
    {
        StringBuilder sb = new();
        sb.AppendLine("Va a realizar entrada del la siguiente mercancía:");
        sb.AppendLine($"Nombre: {SelectedWarehouse!.MerchandiseName}");
        if (SelectedWarehouse!.Packaging is not null)
        {
            sb.AppendLine($"Empaque: {SelectedWarehouse!.Packaging!.Value} {SelectedWarehouse!.Packaging!.Unit}");
        }
        sb.AppendLine($"Existencia: {SelectedWarehouse!.Quantity}");
        sb.AppendLine("Especifique la cantidad:");

        var addQuantity = await Shell.Current.DisplayPromptAsync("Entrada de mercancía", sb.ToString(), "Entrada", "Cancelar", "0", 10, Keyboard.Numeric);

        if (string.IsNullOrEmpty(addQuantity) || !double.TryParse(addQuantity, out double theQuantity))
        {
            SelectedWarehouse = null;
            return;
        }

        var result = await articlesForWarehouseServ.UpdateAsync(serverURL, new() { MerchandiseId = SelectedWarehouse!.MerchandiseId, Quantity = SelectedWarehouse!.Quantity + theQuantity });

        if (result)
        {
            int idx = Warehouses!.IndexOf(SelectedWarehouse);
            SelectedWarehouse.Quantity += theQuantity;
            Warehouses[idx] = SelectedWarehouse;
        }
    }

    [RelayCommand]
    async Task ShowAddMerchandise()
    {
        IsActive = true;
        var categories = await merchandiseServ.GetAllCategoriesAsync(serverURL);
        if (categories.Any())
        {
            Dictionary<string, object> sendData = new() { { "Categories", categories.ToList() } };
            await Shell.Current.GoToAsync(nameof(PgAddEditWarehouse), true, sendData);
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(PgAddEditWarehouse), true);
        }
    }

    [RelayCommand]
    async Task ShowEditMerchandise()
    {
        IsActive = true;
        Dictionary<string, object> sendData = [];
        var merchandise = await merchandiseServ.GetByIdAsync(serverURL, SelectedWarehouse!.MerchandiseId!);
        sendData.Add("CurrentMerchandise", merchandise!);
        var categories = await merchandiseServ.GetAllCategoriesAsync(serverURL);
        if (categories.Any())
        {
            sendData.Add("Categories", categories.ToList());
        }
        await Shell.Current.GoToAsync(nameof(PgAddEditWarehouse), true, sendData);
    }

    [RelayCommand]
    async Task DeleteMerchandise()
    {
        //todo: crear objetos dto para mostrar la siguiente información
        var merchandise = await merchandiseServ.GetByIdAsync(serverURL, SelectedWarehouse!.MerchandiseId!);
        var articleforsale = await articlesForSalesServ.GetByIdAsync(serverURL, SelectedWarehouse!.MerchandiseId!);
        var products = await productsServ.GetAllByMerchandiseId(serverURL, SelectedWarehouse!.MerchandiseId!);
        StringBuilder sb = new();
        sb.AppendLine($"Tenga en cuenta que al eliminar la mercancía del almacén, también se eliminara el articulo, producto y sus ofertas relacionados");
        sb.AppendLine($"INFORMACION:");
        sb.AppendLine($"Nombre: {SelectedWarehouse!.MerchandiseName}");
        sb.AppendLine($"Categoría: {merchandise!.Category}");
        sb.AppendLine(merchandise!.Packaging is null ? "Presentación: Por Unidad" : $"Presentación: {merchandise!.Packaging.Value:0.00} {merchandise!.Packaging.Unit}");
        if (!string.IsNullOrEmpty(merchandise!.Description))
        {
            sb.AppendLine($"Descripción: {merchandise!.Description}");
        }
        sb.AppendLine($"Existencia en almacén: {SelectedWarehouse.Quantity}");
        sb.AppendLine($"Precio inicial: {articleforsale!.Price}");
        if (products is not null || products!.Any())
        {
            sb.AppendLine($"PRODUCTOS RELACIONADOS Y SUS OFERTAS");
            foreach (var item in products!)
            {
                sb.AppendLine($"Producto: {item.ProductQuantity} - {item.ProductName} {item.ArticlePrice:0.00} " + (item.HasOffers ? "[Con ofertas]" : "[Sin ofertas]"));
            }
        }
        sb.AppendLine($"¿Seguro que quiere eliminar?");
        sb.AppendLine("");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar mercancía", sb.ToString().TrimEnd(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            SelectedWarehouse = null;
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            SelectedWarehouse = null;
            return;
        }

        var result = await merchandiseServ.DeleteAsync(serverURL, SelectedWarehouse!.MerchandiseId!);
        if (result)
        {
            Warehouses!.Remove(SelectedWarehouse);
        }
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvWarehouseViewModel, PgAddWarehouseMessage, string>(this, "AddMerchandise", async (r, m) =>
        {
            var merchandiseId = await merchandiseServ.InsertAsync(serverURL, m.Merchandise);
            if (!string.IsNullOrEmpty(merchandiseId))
            {
                var sussess = await articlesForWarehouseServ.UpdateAsync(serverURL, new() { MerchandiseId = merchandiseId, Quantity = m.Quantity });

                if (sussess)
                {
                    r.Warehouses ??= [];
                    DTO2 itemInsert = new()
                    {
                        MerchandiseId = merchandiseId,
                        MerchandiseName = m.Merchandise.Name,
                        Quantity = m.Quantity,
                        Packaging = m.Merchandise.Packaging
                    };
                    r.Warehouses.Insert(0, itemInsert);
                }
            }

            IsActive = false;
            SelectedWarehouse = null;
        });

        WeakReferenceMessenger.Default.Register<CvWarehouseViewModel, DTO1, string>(this, "EditMerchandise", async (r, m) =>
        {
            var result = await merchandiseServ.UpdateAsync(serverURL, m);

            if (result)
            {
                int idx = r.Warehouses!.IndexOf(r.SelectedWarehouse!);

                r.SelectedWarehouse!.MerchandiseName = m.Name;
                r.SelectedWarehouse!.Packaging = m.Packaging;

                r.Warehouses[idx] = r.SelectedWarehouse!;
            }

            IsActive = false;
            SelectedWarehouse = null;
        });

        WeakReferenceMessenger.Default.Register<CvWarehouseViewModel, string, string>(this, nameof(PgAddEditWarehouse), (r, m) =>
        {
            if (m == "cancel")
            {
                r.SelectedWarehouse = null;
                IsActive = false;
            }
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        IsBusy = true;
        await GetWarehouse();
        IsBusy = false;
    }

    async Task GetWarehouse()
    {
        var getAll = await articlesForWarehouseServ.GetAllAsync(serverURL);
        if (getAll?.Count() == 0)
        {
            return;
        }
        Warehouses = new(getAll!);
    }
    #endregion
}
