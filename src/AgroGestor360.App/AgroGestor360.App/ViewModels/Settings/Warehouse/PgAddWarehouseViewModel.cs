using AgroGestor360.App.Services;
using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Categories), nameof(Categories))]
public partial class PgAddWarehouseViewModel : ObservableValidator
{
    readonly IMeasurementService measurementServ;

    public PgAddWarehouseViewModel(IMeasurementService measurementService)
    {
        measurementServ = measurementService;
        Magnitudes = new(measurementServ.GetMeasurementNames());
        SelectedMagnitude = Magnitudes.First();
    }

    [ObservableProperty]
    List<MerchandiseCategory>? categories;

    [ObservableProperty]
    MerchandiseCategory? selectedCategory;

    [ObservableProperty]
    string? newCategory;

    [ObservableProperty]
    bool isSetNewCategory = true;

    [ObservableProperty]
    [Required]
    [MinLength(3)]
    string? name;

    [ObservableProperty]
    [Required]
    string? quantity;

    [ObservableProperty]
    List<string>? magnitudes;

    [ObservableProperty]
    string? selectedMagnitude;

    [ObservableProperty]
    ObservableCollection<string>? units;

    [ObservableProperty]
    string? selectedUnit;

    [ObservableProperty]
    string? value;

    [ObservableProperty]
    string? description;

    [ObservableProperty]
    bool isUnit;

    [ObservableProperty]
    bool isVisisbleInfo;

    [RelayCommand]
    void SetNewCategory()
    {
        IsSetNewCategory = !IsSetNewCategory;
        if (IsSetNewCategory)
        {
            SelectedCategory = null;
        }
        if (!IsSetNewCategory)
        {
            NewCategory = null;
        }
    }

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..", true);

    [RelayCommand]
    async Task Add()
    {
        ValidateAllProperties();

        double theQuantity = 0;
        double theValue = 0;

        if (HasErrors || !double.TryParse(Quantity, out theQuantity) || (!IsUnit && !double.TryParse(Value, out theValue)))
        {
            IsVisisbleInfo = true;
            await Task.Delay(3000);
            IsVisisbleInfo = false;

            return;
        }

        DTO1 merchandise = new()
        {
            Name = Name!.Trim().ToUpper(),
            Category = string.IsNullOrEmpty(NewCategory) ? SelectedCategory : new() { Name = NewCategory.Trim().ToUpper()},
            Description = Description?.Trim().ToUpper(),
            Packaging = IsUnit ? null : new() { Measure = SelectedMagnitude, Unit = SelectedUnit, Value = theValue }
        };

        WeakReferenceMessenger.Default.Send(new DTO2() { Merchandise = merchandise, Quantity = theQuantity }, "AddWarehouse");

        await Cancel();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Categories))
        {
            if (Categories?.Any() ?? false)
            {
                IsSetNewCategory = false; 
            }
        }

        if (e.PropertyName == nameof(SelectedMagnitude))
        {
            if (string.IsNullOrEmpty(SelectedMagnitude))
            {
                return;
            }
            var getUnits = measurementServ.GetNamesAndUnitsMeasurement(SelectedMagnitude);
            Units = !getUnits?.Any() ?? true ? null : new(getUnits!);
            SelectedUnit = Units?.First();
            IsUnit = SelectedUnit is null;
        }
    }
}
