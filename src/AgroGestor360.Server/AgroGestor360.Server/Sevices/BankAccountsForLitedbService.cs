using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public class BankAccountsForLitedbService
{
    readonly ILiteCollection<BankAccount> collection;

    public BankAccountsForLitedbService(BanksDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<BankAccount>();
    }

    public bool Exists => collection.Count() > 0;

    public BankAccount GetAllById(ObjectId id) => collection.FindById(id);

    public string Insert(BankAccount account) => collection.Insert(account).AsString;

    public bool Update(BankAccount account) => collection.Update(account);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
