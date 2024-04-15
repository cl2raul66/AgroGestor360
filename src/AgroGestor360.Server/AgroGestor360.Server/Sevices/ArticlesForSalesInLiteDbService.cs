using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IArticlesForSalesInLiteDbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<ArticleItemForSale> GetAll();
    IEnumerable<MerchandiseCategory> GetAllCategories();
    IEnumerable<MerchandiseItem> GetAllMerchandise();
    ArticleItemForSale GetById(ObjectId id);
    string Insert(ArticleItemForSale entity);
    bool Update(ArticleItemForSale entity);
}

public class ArticlesForSalesInLiteDbService : IArticlesForSalesInLiteDbService
{
    readonly ILiteCollection<ArticleItemForSale> collection;

    public ArticlesForSalesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Articles_Sales")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<ArticleItemForSale>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ArticleItemForSale> GetAll() => collection.FindAll();

    public IEnumerable<MerchandiseItem> GetAllMerchandise() => collection.Find(Query.All())?.Select(x => x.Merchandise ?? new()) ?? [];

    public IEnumerable<MerchandiseCategory> GetAllCategories() => collection.Find(Query.All())?.Select(x => x.Merchandise?.Category ?? new()).DistinctBy(c => c?.Name).Where(c => c != null) ?? [];

    public ArticleItemForSale GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ArticleItemForSale entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ArticleItemForSale entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
