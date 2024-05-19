using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IWasteInvoicesInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(Guid code);
    IEnumerable<Invoice> GetAll();
    Invoice GetById(Guid code);
    string Insert(Invoice entity);
    void Rollback();
    bool Update(Invoice entity);
}

public class WasteInvoicesInLiteDbService : IWasteInvoicesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Invoice> collection;

    public WasteInvoicesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Invoices_Waste")
        };
        var mapper = new BsonMapper();

        mapper.Entity<Invoice>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<Invoice>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public Invoice GetById(Guid code) => collection.FindById(code);

    public IEnumerable<Invoice> GetAll() => collection.FindAll();

    public string Insert(Invoice entity) => collection.Insert(entity).AsGuid.ToString();

    public bool Update(Invoice entity) => collection.Update(entity);

    public bool Delete(Guid code) => collection.Delete(code);
}
