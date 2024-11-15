using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IBankAccountsLiteDbService : IDisposable
{
    bool Exist { get; }

    bool Delete(string id);
    bool DeleteBankItem(string id);
    IEnumerable<BankAccount> GetAll();
    IEnumerable<BankItem> GetBankItemAll();
    BankItem GetBankItemById(string id);
    BankAccount GetById(string id);
    string Insert(BankAccount entity);
    string InsertBankItem(BankItem entity);
    bool Update(BankAccount entity);
    bool UpdateBankItem(BankItem entity);
}

public class BankAccountsLiteDbService : IBankAccountsLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<BankAccount> collection;
    readonly ILiteCollection<BankItem> collection1;

    public BankAccountsLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("BankAccounts")
        };

        var mapper = new BsonMapper();

        mapper.Entity<BankAccount>().Id(x => x.Number);

        db = new(cnx, mapper);
        collection = db.GetCollection<BankAccount>();
        collection1 = db.GetCollection<BankItem>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

    public bool Exist => collection.Count() > 0;

    public IEnumerable<BankAccount> GetAll() => collection.FindAll();

    public BankAccount GetById(string id) => collection.FindById(id);

    public string Insert(BankAccount entity) => collection.Insert(entity).AsString;

    public bool Update(BankAccount entity) => collection.Update(entity);

    public bool Delete(string id) => collection.Delete(id);


    public IEnumerable<BankItem> GetBankItemAll() => collection1.FindAll();

    public BankItem GetBankItemById(string id) => collection1.FindById(id);

    public string InsertBankItem(BankItem entity) => collection1.Insert(entity).AsString;

    public bool UpdateBankItem(BankItem entity) => collection1.Update(entity);

    public bool DeleteBankItem(string id) => collection1.Delete(id);
}
