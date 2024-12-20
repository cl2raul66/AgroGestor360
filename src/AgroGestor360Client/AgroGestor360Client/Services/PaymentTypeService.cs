using AgroGestor360Client.Tools;

namespace AgroGestor360Client.Services
{
    public interface IPaymentTypeService
    {
        IEnumerable<string> GetAll();
        PaymentType GetByName(string name);
        string GetNameByType(PaymentType type);
    }

    public class PaymentTypeService : IPaymentTypeService
    {
        private static readonly Dictionary<PaymentType, string> translations = new()
        {
            { PaymentType.Check, "Cheque" },
            { PaymentType.CreditCard, "Tarjeta de crédito" },
            { PaymentType.Cash, "Efectivo" },
            { PaymentType.DebitCard, "Tarjeta de débito" },
            { PaymentType.BankTransfer, "Transferencia bancaria" },
            { PaymentType.BankDeposit, "Depósito bancario" }
        };

        public IEnumerable<string> GetAll()
        {
            return translations.Values;
        }

        public PaymentType GetByName(string name)
        {
            return translations.First(x => x.Value == name).Key;
        }

        public string GetNameByType(PaymentType type)
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
