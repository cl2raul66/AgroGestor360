using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IWasteQuotationInLiteDbService
{
    bool Delete(Guid code);
    IEnumerable<Quotation> GetAll();
    Quotation GetById(Guid code);
    string Insert(Quotation entity);
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

    public Quotation GetById(Guid code) => collection.FindById(code);

    public IEnumerable<Quotation> GetAll() => collection.FindAll();

    public string Insert(Quotation entity) => collection.Insert(entity).AsGuid.ToString();

    public bool Delete(Guid code) => collection.Delete(code);
}
