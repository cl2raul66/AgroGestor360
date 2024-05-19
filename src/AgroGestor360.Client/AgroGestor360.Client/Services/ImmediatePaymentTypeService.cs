using AgroGestor360.Client.Tools;

namespace AgroGestor360.Client.Services
{
    public interface IImmediatePaymentTypeService
    {
        IEnumerable<string> GetAll();
        ImmediatePaymentType GetByName(string name);
        string GetNameByType(ImmediatePaymentType type);
    }

    public class ImmediatePaymentTypeService : IImmediatePaymentTypeService
    {
        private static readonly Dictionary<ImmediatePaymentType, string> translations = new()
        {
            { ImmediatePaymentType.Cash, "Efectivo" },
            { ImmediatePaymentType.Card, "Tarjeta" },
            { ImmediatePaymentType.BankTransfer, "Transferencia Bancaria" },
            { ImmediatePaymentType.MobilePayment, "Pago Móvil" },
            { ImmediatePaymentType.CustomerAccount, "Cuenta del Cliente" }
        };

        public IEnumerable<string> GetAll()
        {
            return translations.Values;
        }

        public ImmediatePaymentType GetByName(string name)
        {
            return translations.First(x => x.Value == name).Key;
        }

        public string GetNameByType(ImmediatePaymentType type)
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
