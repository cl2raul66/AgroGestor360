using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IWasteQuotationInLiteDbService : IDisposable
{
    void BeginTrans();
    void Commit();
    bool Delete(string code);
    IEnumerable<Quotation> GetAll();
    Quotation GetByCode(string code);
    string Insert(Quotation entity);
    bool InsertMany(IEnumerable<Quotation> entities);
    void Rollback();
}

public class WasteQuotationInLiteDbService : IWasteQuotationInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Quotation> collection;

    public WasteQuotationInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Quotes_Waste")
        };
        var mapper = new BsonMapper();

        mapper.Entity<Quotation>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<Quotation>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();


    public Quotation GetByCode(string code) => collection.FindById(code);

    public IEnumerable<Quotation> GetAll() => collection.FindAll();

    public string Insert(Quotation entity) => collection.Insert(entity).AsString;

    public bool InsertMany(IEnumerable<Quotation> entities) => collection.InsertBulk(entities) == entities.Count();

    public bool Delete(string code) => collection.Delete(code);
}
