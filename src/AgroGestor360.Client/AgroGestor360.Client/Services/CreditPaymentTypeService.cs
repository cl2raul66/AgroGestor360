using AgroGestor360.Client.Tools;

namespace AgroGestor360.Client.Services
{
    public interface ICreditPaymentTypeService
    {
        IEnumerable<string> GetAll();
        CreditPaymentType GetByName(string name);
        string GetNameByType(CreditPaymentType type);
    }

    public class CreditPaymentTypeService : ICreditPaymentTypeService
    {
        private static readonly Dictionary<CreditPaymentType, string> translations = new()
        {
            { CreditPaymentType.Check, "Cheque" },
            { CreditPaymentType.CreditCard, "Tarjeta de Crédito" },
            { CreditPaymentType.CustomerAccount, "Cuenta del Cliente" }
        };

        public IEnumerable<string> GetAll()
        {
            return translations.Values;
        }

        public CreditPaymentType GetByName(string name)
        {
            return translations.First(x => x.Value == name).Key;
        }

        public string GetNameByType(CreditPaymentType type)
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
}
