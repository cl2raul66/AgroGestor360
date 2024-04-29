using AgroGestor360.App.Models;
using AgroGestor360.App.Services;
using AgroGestor360.App.Views.Settings.Warehouse;
using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(Categories), nameof(Categories))]
[QueryProperty(nameof(CurrentMerchandise), nameof(CurrentMerchandise))]
public partial class PgAddEditWarehouseViewModel : ObservableValidator
{
    readonly IMeasurementService measurementServ;

    public PgAddEditWarehouseViewModel(IMeasurementService measurementService)
    {
        measurementServ = measurementService;
        Magnitudes = new(measurementServ.GetMeasurementNames());
        SelectedMagnitude = Magnitudes.First();
    }

    [ObservableProperty]
    DTO1? currentMerchandise;

    [ObservableProperty]
    List<string>? categories;

    [ObservableProperty]
    string? selectedCategory;

    [ObservableProperty]
    string? newCategory;

    [ObservableProperty]
    bool isSetNewCategory = true;

    [ObservableProperty]
    [Required]
    [MinLength(3)]
    string? name;

    [ObservableProperty]
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
    async Task Cancel()
    {
        WeakReferenceMessenger.Default.Send("cancel", nameof(PgAddEditWarehouse));
        await Shell.Current.GoToAsync("..", true);
    }

    [RelayCommand]
    async Task AddOrEdit()
    {
        ValidateAllProperties();

        if (CurrentMerchandise is null)
        {
            double theQuantity = 0;
            double theValue = 0;

            if (HasErrors || (!double.TryParse(Quantity, out theQuantity)) || (!IsUnit && !double.TryParse(Value, out theValue)))
            {
                IsVisisbleInfo = true;
                await Task.Delay(3000);
                IsVisisbleInfo = false;

                return;
            }

            DTO1 merchandise = new()
            {
                Name = Name!.Trim().ToUpper(),
                Category = string.IsNullOrEmpty(NewCategory) ? SelectedCategory : NewCategory.Trim().ToUpper(),
                Description = Description?.Trim().ToUpper(),
                Packaging = IsUnit ? null : new() { Measure = SelectedMagnitude, Unit = SelectedUnit, Value = theValue }
            };

            WeakReferenceMessenger.Default.Send(new PgAddWarehouseMessage(merchandise, theQuantity), "AddMerchandise");
        }
        else
        {
            double theValue = 0;

            if (HasErrors || (!IsUnit && !double.TryParse(Value, out theValue)))
            {
                IsVisisbleInfo = true;
                await Task.Delay(3000);
                IsVisisbleInfo = false;

                return;
            }
            DTO1 merchandise = new()
            {
                Id = CurrentMerchandise.Id,
                Name = Name!.Trim().ToUpper(),
                Category = string.IsNullOrEmpty(NewCategory) ? SelectedCategory : NewCategory.Trim().ToUpper(),
                Description = Description?.Trim().ToUpper(),
                Packaging = IsUnit ? null : new() { Measure = SelectedMagnitude, Unit = SelectedUnit, Value = theValue }
            };
            WeakReferenceMessenger.Default.Send(merchandise, "EditMerchandise");
        }

        await Shell.Current.GoToAsync("..", true);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(CurrentMerchandise))
        {
            if (CurrentMerchandise is not null)
            {
                Name = CurrentMerchandise?.Name;
                SelectedCategory = Categories?.FirstOrDefault(x => x == CurrentMerchandise?.Category);
                Description = CurrentMerchandise?.Description;
                if (CurrentMerchandise?.Packaging is not null)
                {
                    SelectedMagnitude = CurrentMerchandise.Packaging.Measure;
                    SelectedUnit = CurrentMerchandise.Packaging.Unit;
                    Value = CurrentMerchandise.Packaging.Value.ToString();
                }
            }
        }

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
