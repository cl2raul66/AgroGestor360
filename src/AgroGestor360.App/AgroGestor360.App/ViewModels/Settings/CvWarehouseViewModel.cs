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
    readonly IMerchandiseCategoryService merchandiseCategoryServ;
    readonly IMerchandiseService merchandiseServ;
    readonly IWarehouseService warehouseServ;
    readonly IAuthService authServ;
    readonly string serverURL;

    public CvWarehouseViewModel(IMerchandiseCategoryService merchandiseCategoryService, IWarehouseService warehouseService, IAuthService authService, IMerchandiseService merchandiseService)
    {
        merchandiseCategoryServ = merchandiseCategoryService;
        merchandiseServ = merchandiseService;
        warehouseServ = warehouseService;
        authServ = authService;
        serverURL = Preferences.Default.Get("serverurl", string.Empty);
        IsActive = true;
    }

    [ObservableProperty]
    ObservableCollection<MerchandiseCategory>? categories;

    [ObservableProperty]
    MerchandiseCategory? selectedCategory;
    
    [ObservableProperty]
    ObservableCollection<WarehouseItemGet>? warehouses;

    [ObservableProperty]
    WarehouseItemGet? selectedWarehouse;

    [RelayCommand]
    async Task AddCategory()
    {
        string categoryName = await Shell.Current.DisplayPromptAsync("Agregar categoría", "Nombre:", "Agregar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(categoryName) || string.IsNullOrWhiteSpace(categoryName))
        {
            return;
        }

        MerchandiseCategory newCategory = new() { Name = categoryName.Trim().ToUpper() };

        var id = await merchandiseCategoryServ.InsertAsync(serverURL, newCategory);

        if (!string.IsNullOrEmpty(id))
        {
            Categories ??= [];
            newCategory.Id = id;
            Categories.Insert(0, newCategory);
        }
    }

    [RelayCommand]
    async Task RemoveCategory()
    {
        MerchandiseCategory current = SelectedCategory!;
        SelectedCategory = null;

        StringBuilder sb = new();
        sb.AppendLine($"¿Seguro que quiere eliminar esta categoría: {current!.Name}?");
        sb.AppendLine("Inserte la contraseña:");
        var pwd = await Shell.Current.DisplayPromptAsync("Eliminar categoría", sb.ToString(), "Autenticar y eliminar", "Cancelar", "Escriba aquí");
        if (string.IsNullOrEmpty(pwd) || string.IsNullOrWhiteSpace(pwd))
        {
            return;
        }

        var approved = await authServ.AuthRoot(serverURL, pwd);
        if (!approved)
        {
            await Shell.Current.DisplayAlert("Error", "¡Contraseña incorrecta!", "Cerrar");
            return;
        }

        var result = await merchandiseCategoryServ.DeleteAsync(serverURL, current!.Id!);
        if (result)
        {
            Categories!.Remove(current);
        }
    }

    [RelayCommand]
    async Task AddWarehouse()
    {
        Dictionary<string, object> sendData = new() { { "CurrentCategory", SelectedCategory!.Name! } };
        await Shell.Current.GoToAsync(nameof(PgAddWarehouse), true, sendData);
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        WeakReferenceMessenger.Default.Register<CvWarehouseViewModel, WarehouseItemGet, string>(this, "AddWarehouse", async (r, m) =>
        {
            Merchandise merchandise = new() { Name = m.Name, MerchandiseCategory = SelectedCategory!.Id, Packaging = m.Packaging, Description = m.Description };

            var merchandiseId = await merchandiseServ.InsertAsync(serverURL, merchandise);

            if (!string.IsNullOrEmpty(merchandiseId))
            {
                var result = await warehouseServ.InsertAsync(serverURL, new() { MerchandiseId = merchandiseId, Quantity = m.Quantity });

                if (!string.IsNullOrEmpty(result))
                {
                    r.Warehouses ??= [];
                    r.Warehouses.Insert(0, m);
                }
            }
            SelectedCategory = null;
        });
    }

    #region EXTRA
    public async void Initialize()
    {
        await Task.WhenAll(GetCategories(), GetWarehouse());
    }

    async Task GetCategories()
    {
        bool exist = await merchandiseCategoryServ.CheckExistence(serverURL);
        if (exist)
        {
            var getcategories = await merchandiseCategoryServ.GetAllAsync(serverURL);
            Categories = new(getcategories!);
        }
    }

    async Task GetWarehouse()
    {
        bool exist = await warehouseServ.CheckExistence(serverURL);
        if (exist)
        {
            var getAll = await warehouseServ.GetAllAsync(serverURL);
            Warehouses = new(getAll!);
        }
    }
    #endregion
}
