using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IAccountsReceivableInLiteDbService  : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Deleted(ObjectId id);
    IEnumerable<AccountReceivableRecord> GetAll();
    IEnumerable<AccountReceivableRecord> GetAllByDateRange(DateTime startDate, DateTime endDate);
    AccountReceivableRecord GetById(ObjectId id);
    string Insert(AccountReceivableRecord entity);
    void Rollback();
}

public class AccountsReceivableInLiteDbService : IAccountsReceivableInLiteDbService
{
    readonly ILiteCollection<AccountReceivableRecord> collection;
    readonly LiteDatabase db;

    public AccountsReceivableInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("AccountsReceivable")
        };

        var mapper = new BsonMapper();

        db = new(cnx, mapper);
        collection = db.GetCollection<AccountReceivableRecord>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public IEnumerable<AccountReceivableRecord> GetAll() => collection.FindAll();

    public IEnumerable<AccountReceivableRecord> GetAllByDateRange(DateTime startDate, DateTime endDate) => collection.Find(x => x.DateOfPayment >= startDate && x.DateOfPayment <= endDate);

    public AccountReceivableRecord GetById(ObjectId id) => collection.FindById(id);

    public string Insert(AccountReceivableRecord entity)
    {
        entity.Id = ObjectId.NewObjectId();
        return collection.Insert(entity).AsObjectId?.ToString() ?? string.Empty;
    }

    public bool Deleted(ObjectId id) => collection.Delete(id);

    public void Dispose() => db.Dispose();
}
