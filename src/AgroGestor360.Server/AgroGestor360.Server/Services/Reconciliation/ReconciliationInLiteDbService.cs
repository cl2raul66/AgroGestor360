using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public class ReconciliationInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<ReconciliationPolicy> policies;
    readonly ILiteCollection<CashReconciliation> collection;

    public ReconciliationInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Reconciliation")
        };

        db = new(cnx);

        policies = db.GetCollection<ReconciliationPolicy>();

        collection = db.GetCollection<CashReconciliation>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    #region POLICIES
    public bool HasPolitic() => policies.Count() > 0;

    public ReconciliationPolicy GetPolicy() => policies.FindById(0);

    public int InsertPolicy(ReconciliationPolicy entity)
    {
        if (policies.Count() == 0)
        {
            entity.Id = 1;
        }

        return policies.Insert(entity).AsInt32;
    }

    public bool UpdatePolicy(ReconciliationPolicy entity) => policies.Update(entity);
    #endregion
}
