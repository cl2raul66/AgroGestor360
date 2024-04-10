using AgroGestor360.App.Services;
using AgroGestor360.Client.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgroGestor360.App.ViewModels;

[QueryProperty(nameof(CurrentCategory), nameof(CurrentCategory))]
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
    string? currentCategory;

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

        WeakReferenceMessenger.Default.Send(new WarehouseItemGet()
        {
            Name = Name!.Trim().ToUpper(),
            Packaging = IsUnit ? null : new() { Measure = SelectedMagnitude!.ToUpper(), Unit = SelectedUnit!.ToUpper(), Value = theValue },
            Quantity = theQuantity,
            Description = Description?.Trim().ToUpper()
        }, "AddWarehouse");

        await Cancel();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(SelectedMagnitude))
        {
            if (string.IsNullOrEmpty(SelectedMagnitude))
            {
                return;
            }
            Units = new(measurementServ.GetNamesAndUnitsMeasurement(SelectedMagnitude!));
            SelectedUnit = Units.First();
        }
    }
}
