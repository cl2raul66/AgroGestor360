using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IWasteInvoicesInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    IEnumerable<WasteInvoice> GetAll();
    WasteInvoice GetById(string code);
    string Insert(WasteInvoice entity);
    void Rollback();
    bool Update(WasteInvoice entity);
}

public class WasteInvoicesInLiteDbService : IWasteInvoicesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<WasteInvoice> collection;

    public WasteInvoicesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Invoices_Waste")
        };
        var mapper = new BsonMapper();

        mapper.Entity<WasteInvoice>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<WasteInvoice>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public WasteInvoice GetById(string code) => collection.FindById(code);

    public IEnumerable<WasteInvoice> GetAll() => collection.FindAll();

    public string Insert(WasteInvoice entity) => collection.Insert(entity).AsString;

    public bool Update(WasteInvoice entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);
}
