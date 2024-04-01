using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IBanksForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<Bank> GetAll();
    Bank GetById(ObjectId id);
    string Insert(Bank bank);
    bool Update(Bank bank);
}

public class BanksForLitedbService : IBanksForLitedbService
{
    readonly ILiteCollection<Bank> collection;

    public BanksForLitedbService(BanksDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<Bank>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<Bank> GetAll() => collection.FindAll();

    public Bank GetById(ObjectId id) => collection.FindById(id);

    public string Insert(Bank bank) => collection.Insert(bank).AsString;

    public bool Update(Bank bank) => collection.Update(bank);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
