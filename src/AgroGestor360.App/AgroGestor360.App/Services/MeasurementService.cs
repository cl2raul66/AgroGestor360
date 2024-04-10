using UnitsNet;
using UnitsNet.Units;

namespace AgroGestor360.App.Services;

public interface IMeasurementService
{
    IEnumerable<string> GetMeasurementNames();
    IEnumerable<string> GetNamesUnits(string measurement);
    IEnumerable<string> GetNamesAndUnitsMeasurement(string measurement);
}

public class MeasurementService : IMeasurementService
{

    readonly Dictionary<string, string> measurementAll = new() {
        {nameof(Area), "Area"},
        {nameof(Mass), "Masa"},
        {nameof(Length), "Longitud"},
        {nameof(Volume), "Volumen"},
    };

    readonly Dictionary<string, string> areaUnit = new()
    {
        {nameof(AreaUnit.Acre),"Acre"},
        {nameof(AreaUnit.Hectare),"Hectárea"},
        {nameof(AreaUnit.SquareCentimeter),"Centímetro cuadrado"},
        {nameof(AreaUnit.SquareMeter),"Metro cuadrado"},
        {nameof(AreaUnit.SquareMillimeter),"Milímetro cuadrado"},
        {nameof(AreaUnit.SquareFoot),"Pie cuadrado"},
        {nameof(AreaUnit.SquareInch),"Pulgada cuadrada"},
        {nameof(AreaUnit.SquareYard),"Yarda cuadrada"}
    };

    readonly Dictionary<string, string> massUnit = new()
    {
        {nameof(MassUnit.Gram),"Gramo"},
        {nameof(MassUnit.Milligram),"Miligramo"},
        {nameof(MassUnit.Kilogram),"Kilogramo"},
        {nameof(MassUnit.Pound),"Libra"},
        {nameof(MassUnit.Ounce),"Onza"}
    };

    readonly Dictionary<string, string> lengthUnit = new()
    {
        {nameof(LengthUnit.Centimeter),"Centímetro"},
        {nameof(LengthUnit.Meter),"Metro"},
        {nameof(LengthUnit.Millimeter),"Milímetro"},
        {nameof(LengthUnit.Foot),"Pie"},
        {nameof(LengthUnit.Inch),"Pulgada"},
        {nameof(LengthUnit.Yard),"Yarda"}
    };

    readonly Dictionary<string, string> volumeUnit = new()
    {
        {nameof(VolumeUnit.Milliliter),"Mililitro"},
        {nameof(VolumeUnit.Liter),"Litro"},
        {nameof(VolumeUnit.CubicMeter),"Metro cúbico"},
        {nameof(VolumeUnit.CubicCentimeter),"Centímetro cúbico"},
        {nameof(VolumeUnit.CubicFoot),"Pie cúbico"},
        {nameof(VolumeUnit.CubicInch),"Pulgada cúbica"},
        {nameof(VolumeUnit.UsGallon),"Galón"},
        {nameof(VolumeUnit.UsOunce),"Onza"}
    };

    public IEnumerable<string> GetMeasurementNames() => measurementAll.Values;

    public IEnumerable<string> GetNamesUnits(string measurement) => measurement switch
    {
        "Area" => areaUnit.Values,
        "Masa" => massUnit.Values,
        "Longitud" => lengthUnit.Values,
        "Volumen" => volumeUnit.Values,
        _ => Array.Empty<string>(),
    };

    public IEnumerable<string> GetNamesAndUnitsMeasurement(string measurement)
    {
        var result = measurement switch
        {
            "Area" => areaUnit.Select(x => $"{x.Value} [{Area.GetAbbreviation(Enum.Parse<AreaUnit>(x.Key))}]"),
            "Masa" => massUnit.Select(x => $"{x.Value} [{Mass.GetAbbreviation(Enum.Parse<MassUnit>(x.Key))}]"),
            "Longitud" => lengthUnit.Select(x => $"{x.Value} [{Length.GetAbbreviation(Enum.Parse<LengthUnit>(x.Key))}]"),
            "Volumen" => volumeUnit.Select(x => $"{x.Value} [{Volume.GetAbbreviation(Enum.Parse<VolumeUnit>(x.Key))}]"),
            _ => [],
        };
        return result;
    }

}
