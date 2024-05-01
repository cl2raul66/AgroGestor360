using AgroGestor360.Client.Tools;

namespace AgroGestor360.Client.Services;

public interface IFinancialInstrumentTypeService
{
    IEnumerable<string> GetAll();
    FinancialInstrumentType? GetByName(string name);
    string GetNameByType(FinancialInstrumentType type);
}

public class FinancialInstrumentTypeService : IFinancialInstrumentTypeService
{
    private static readonly Dictionary<FinancialInstrumentType, string> translations = new()
    {
        { FinancialInstrumentType.Current, "Cuenta Corriente" },
        { FinancialInstrumentType.Savings, "Cuenta de Ahorros" },
        { FinancialInstrumentType.Investment, "Cuenta de Inversión" },
        { FinancialInstrumentType.Payroll, "Cuenta de Nómina" },
        { FinancialInstrumentType.CreditCard, "Tarjeta de Crédito" },
        { FinancialInstrumentType.DebitCard, "Tarjeta de Débito" }
    };

    public IEnumerable<string> GetAll()
    {
        return translations.Values;
    }

    public FinancialInstrumentType? GetByName(string name)
    {
        return translations.FirstOrDefault(x => x.Value == name).Key;
    }

    public string GetNameByType(FinancialInstrumentType type)
    {
        if (translations.TryGetValue(type, out var translation))
        {
            return translation;
        }
        else
        {
            return string.Empty;
        }
    }

}
