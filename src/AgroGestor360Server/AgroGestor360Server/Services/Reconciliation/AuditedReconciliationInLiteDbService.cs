using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface IAuditedReconciliationInLiteDbService : IDisposable
{
    void BeginTrans();
    void Commit();
    CashReconciliation GetById(string id);
    void Rollback();
}

public class AuditedReconciliationInLiteDbService : IAuditedReconciliationInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<CashReconciliation> collection;

    public AuditedReconciliationInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("AuditedReconciliation")
        };

        db = new(cnx);

        collection = db.GetCollection<CashReconciliation>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public CashReconciliation GetById(string id) => collection.FindById(id);

    public void Dispose() => db.Dispose();
}
