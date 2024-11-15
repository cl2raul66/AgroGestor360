using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IQuotesInLiteDbService : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    bool DeleteMany(IEnumerable<string> codes);
    IEnumerable<Quotation> GetAll();
    Quotation GetByCode(string code);
    IEnumerable<Quotation> GetExpiringQuotes(int expiryDays);
    string Insert(Quotation entity);
    void Rollback();
    bool Update(Quotation entity);
}

public class QuotesInLiteDbService : IQuotesInLiteDbService
{
    readonly LiteDatabase db;
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

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();


    public bool Exist => collection.Count() > 0;

    public Quotation GetByCode(string code) => collection.FindById(code);

    public IEnumerable<Quotation> GetAll() => collection.FindAll();

    public string Insert(Quotation entity) => collection.Insert(entity).AsString;

    public IEnumerable<Quotation> GetExpiringQuotes(int expiryDays)
    {
        var expiryDate = DateTime.Today.AddDays(-expiryDays);
        return collection.Find(q => q.Date <= expiryDate);
    }

    public bool Update(Quotation entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);

    //public bool DeleteMany(IEnumerable<string> codes)
    //{
    //    var count = 0;
    //    foreach (var code in codes)
    //    {
    //        if (!collection.Delete(code))
    //        {
    //            return false;
    //        }
    //        count++;
    //    }
    //    return count == codes.Count();
    //}

    public bool DeleteMany(IEnumerable<string> codes)
    {
        var codesCount = codes.Count();

        var deleted = collection.DeleteMany(Query.In("_id", codes.Select(x => new BsonValue(x))));

        var result = deleted == codesCount;
        return result;
    }
}
