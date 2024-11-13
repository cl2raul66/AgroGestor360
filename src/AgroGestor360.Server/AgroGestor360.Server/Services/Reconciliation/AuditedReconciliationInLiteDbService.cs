using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public class AuditedReconciliationInLiteDbService
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
}
