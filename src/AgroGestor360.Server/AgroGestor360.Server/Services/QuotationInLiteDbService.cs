using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IQuotesInLiteDbService
{
    bool Exist { get; }

    bool Delete(Guid code);
    IEnumerable<Quotation> GetAll();
    Quotation GetById(Guid code);
    string Insert(Quotation quotation);
    bool Update(Quotation quotation);
}

public class QuotesInLiteDbService : IQuotesInLiteDbService
{
    private readonly LiteDatabase db;
    readonly ILiteCollection<Quotation> collection;

    public QuotesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Quotes")
        };
        var mapper = new BsonMapper();

        mapper.Entity<Quotation>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<Quotation>();
        collection.EnsureIndex(x => x.Code);
    }

    public bool Exist => collection.Count() > 0;

    public Quotation GetById(Guid code) => collection.FindById(code);

    public IEnumerable<Quotation> GetAll() => collection.FindAll();

    public string Insert(Quotation quotation) => collection.Insert(quotation).AsGuid.ToString();

    public bool Update(Quotation quotation) => collection.Update(quotation);

    public bool Delete(Guid code) => collection.Delete(code);
}
