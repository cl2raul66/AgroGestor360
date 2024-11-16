using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface IArticlesForSalesInLiteDbService : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(ObjectId id);
    IEnumerable<ArticleItemForSale> GetAll();
    ArticleItemForSale GetById(ObjectId id);
    string Insert(ArticleItemForSale entity);
    void Rollback();
    bool Update(ArticleItemForSale entity);
}

public class ArticlesForSalesInLiteDbService : IArticlesForSalesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<ArticleItemForSale> collection;

    public ArticlesForSalesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Articles_Sales")
        };

        var mapper = new BsonMapper();
        mapper.Entity<ArticleItemForSale>().Id(x => x.MerchandiseId);

        db = new(cnx, mapper);
        collection = db.GetCollection<ArticleItemForSale>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ArticleItemForSale> GetAll() => collection.FindAll();

    public ArticleItemForSale GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ArticleItemForSale entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ArticleItemForSale entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
