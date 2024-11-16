using AgroGestor360Client.Tools;

namespace AgroGestor360Client.Services;

public interface ITypeFrequencyReconciliationPolicyService
{
    IEnumerable<string> GetAll();
    TypeFrequencyReconciliationPolicy? GetByName(string name);
    string GetNameByType(TypeFrequencyReconciliationPolicy type);
}

public class TypeFrequencyReconciliationPolicyService : ITypeFrequencyReconciliationPolicyService
{
    private static readonly Dictionary<TypeFrequencyReconciliationPolicy, string> translations = new()
    {
        { TypeFrequencyReconciliationPolicy.Daily, "Diario" },
        { TypeFrequencyReconciliationPolicy.Weekly, "Semanal" },
        { TypeFrequencyReconciliationPolicy.Monthly, "Mensual" },
        { TypeFrequencyReconciliationPolicy.ShiftChange, "Cambio de Turno" }
    };

    public IEnumerable<string> GetAll()
    {
        return translations.Values;
    }

    public TypeFrequencyReconciliationPolicy? GetByName(string name)
    {
        return translations.FirstOrDefault(x => x.Value == name).Key;
    }

    public string GetNameByType(TypeFrequencyReconciliationPolicy type)
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
