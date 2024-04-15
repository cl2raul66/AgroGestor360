using AgroGestor360.App.Views.Settings.Warehouse;
using AgroGestor360.Client.Models;
using AgroGestor360.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;

namespace AgroGestor360.App.ViewModels;

public partial class CvWarehouseViewModel : ObservableRecipient
{
    readonly IMerchandiseService merchandiseServ;
    readonly IArticlesForWarehouseService articlesForWarehouseServ;
    readonly IArticlesForSalesService articlesForSalesServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvWarehouseViewModel(IArticlesForWarehouseService articlesForWarehouseService, IMerchandiseService merchandiseService, IArticlesForSalesService articlesForSalesService, IAuthService authService)
    {
        merchandiseServ = merchandiseService;
        articlesForWarehouseServ = articlesForWarehouseService;
        articlesForSalesServ = articlesForSalesService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        IsActive = true;
    }

    [ObservableProperty]
    ObservableCollection<MerchandiseCategory>? categories;

    [ObservableProperty]
    MerchandiseCategory? selectedCategory;

    [ObservableProperty]
    ObservableCollection<DTO2_1>? warehouses;

    [ObservableProperty]
    DTO2_1? selectedWarehouse;

    [RelayCommand]
    async Task AddWarehouse()
    {
        var categories = await articlesForWarehouseServ.GetAllCategoriesAsync(serverURL);
        if (categories.Any())
        {
            Dictionary<string, object> sendData = new() { { "Categories", categories.ToList() } };
            await Shell.Current.GoToAsync(nameof(PgAddWarehouse), true, sendData);
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(PgAddWarehouse), true);
        }        
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvWarehouseViewModel, DTO2, string>(this, "AddWarehouse", async (r, m) =>
        {
            var merchandiseId = await merchandiseServ.InsertAsync(serverURL, m.Merchandise!);

            if (!string.IsNullOrEmpty(merchandiseId))
            {
                m.Merchandise!.Id = merchandiseId;
                var warehouseId = await articlesForWarehouseServ.InsertAsync(serverURL, m);

                if (!string.IsNullOrEmpty(warehouseId))
                {
                    var articleId = await articlesForSalesServ.InsertAsync(serverURL, new() { Merchandise = m.Merchandise, Price = 0 });
                    if (!string.IsNullOrEmpty(articleId))
                    {
                        r.Warehouses ??= [];
                        DTO2_1 itemInsert = new()
                        {
                            Id = warehouseId,
                            Name = m.Merchandise!.Name,
                            Category = m.Merchandise!.Category?.Name,
                            Quantity = m.Quantity,
                            Unit = m.Merchandise.Packaging?.Unit,
                            Value = m.Merchandise.Packaging?.Value ?? 0
                        };
                        r.Warehouses.Insert(0, itemInsert);
                    }
                }
            }
            SelectedCategory = null;
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        await GetWarehouse();
    }

    async Task GetWarehouse()
    {
        bool exist = await articlesForWarehouseServ.CheckExistence(serverURL);
        if (exist)
        {
            var getAll = await articlesForWarehouseServ.GetAll1Async(serverURL);
            Warehouses = new(getAll!);
        }
    }
    #endregion
}
