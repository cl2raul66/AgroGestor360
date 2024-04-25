using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IBankAccountsLiteDbService
{
    bool Exist { get; }

    bool Delete(string id);
    IEnumerable<BankAccount> GetAll();
    BankAccount GetById(string id);
    string Insert(BankAccount entity);
    bool Update(BankAccount entity);
}

public class BankAccountsLiteDbService : IBankAccountsLiteDbService
{
    readonly ILiteCollection<BankAccount> collection;

    public BankAccountsLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("BankAccounts")
        };

        var mapper = new BsonMapper();

        mapper.Entity<BankAccount>().Id(x => x.Number);

        LiteDatabase db = new(cnx, mapper);
        collection = db.GetCollection<BankAccount>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<BankAccount> GetAll() => collection.FindAll();

    public BankAccount GetById(string id) => collection.FindById(id);

    public string Insert(BankAccount entity) => collection.Insert(entity).AsString;

    public bool Update(BankAccount entity) => collection.Update(entity);

    public bool Delete(string id) => collection.Delete(id);
}
