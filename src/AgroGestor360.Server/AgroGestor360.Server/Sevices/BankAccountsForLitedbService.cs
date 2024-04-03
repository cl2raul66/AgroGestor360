using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IBankAccountsForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<BankAccount> GetAllDisabled();
    IEnumerable<BankAccount> GetAllEnabled();
    IEnumerable<string> GetAllNumber();
    BankAccount GetById(string id);
    BankAccount? GetByNumber(string number);
    string Insert(BankAccount account);
    bool Update(BankAccount account);
}

public class BankAccountsForLitedbService : IBankAccountsForLitedbService
{
    readonly ILiteCollection<BankAccount> collection;

    public BankAccountsForLitedbService(BanksDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<BankAccount>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<BankAccount> GetAllEnabled() => collection.Find(x => !x.Disabled);

    public IEnumerable<BankAccount> GetAllDisabled() => collection.Find(x => x.Disabled);

    public IEnumerable<string> GetAllNumber() => collection.Find(x => !x.Disabled).Select(x => x.Number ?? string.Empty);

    public BankAccount GetById(string id) => collection.FindById(id);

    public BankAccount? GetByNumber(string number) => collection.Find(x => x.Number == number).FirstOrDefault();

    public string Insert(BankAccount account) => collection.Insert(account).ToString();

    public bool Update(BankAccount account) => collection.Update(account);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
