using UnitsNet;
using UnitsNet.Units;

namespace AgroGestor360App.Services;

public interface IMeasurementService
{
    IEnumerable<string> GetMeasurementNames();
    IEnumerable<string> GetNamesUnits(string measurement);
    IEnumerable<string> GetAbbreviationUnits(string measurement);
    IEnumerable<string> GetNamesAndUnitsMeasurement(string measurement);
}

public class MeasurementService : IMeasurementService
{
    readonly IReadOnlyDictionary<string, string> measurementAll = new Dictionary<string, string>() 
    {
        {"Unit", "Unidad"},
        {nameof(Area), "Area"},
        {nameof(Mass), "Masa"},
        {nameof(Length), "Longitud"},
        {nameof(Volume), "Volumen"}
    };

    readonly IReadOnlyDictionary<string, string> areaUnit = new Dictionary<string, string>()
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

    readonly IReadOnlyDictionary<string, string> massUnit = new Dictionary<string, string>()
    {
        {nameof(MassUnit.Gram),"Gramo"},
        {nameof(MassUnit.Milligram),"Miligramo"},
        {nameof(MassUnit.Kilogram),"Kilogramo"},
        {nameof(MassUnit.Pound),"Libra"},
        {nameof(MassUnit.Ounce),"Onza"}
    };

    readonly IReadOnlyDictionary<string, string> lengthUnit = new Dictionary<string, string>()
    {
        {nameof(LengthUnit.Centimeter),"Centímetro"},
        {nameof(LengthUnit.Meter),"Metro"},
        {nameof(LengthUnit.Millimeter),"Milímetro"},
        {nameof(LengthUnit.Foot),"Pie"},
        {nameof(LengthUnit.Inch),"Pulgada"},
        {nameof(LengthUnit.Yard),"Yarda"}
    };

    readonly IReadOnlyDictionary<string, string> volumeUnit = new Dictionary<string, string>()
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
        "Area" => [.. areaUnit.Values],
        "Masa" => [.. massUnit.Values],
        "Longitud" => [.. lengthUnit.Values],
        "Volumen" => [.. volumeUnit.Values],
        _ => [],
    };

    public IEnumerable<string> GetAbbreviationUnits(string measurement) => measurement switch
    {
        "Area" => areaUnit.Select(x => Area.GetAbbreviation(Enum.Parse<AreaUnit>(x.Key))),
        "Masa" => massUnit.Select(x => Mass.GetAbbreviation(Enum.Parse<MassUnit>(x.Key))),
        "Longitud" => lengthUnit.Select(x => Length.GetAbbreviation(Enum.Parse<LengthUnit>(x.Key))),
        "Volumen" => volumeUnit.Select(x => Volume.GetAbbreviation(Enum.Parse<VolumeUnit>(x.Key))),
        _ => [],
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
