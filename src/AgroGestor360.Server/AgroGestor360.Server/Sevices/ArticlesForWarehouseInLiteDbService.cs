using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IArticlesForWarehouseInLiteDbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<ArticleItemForWarehouse> GetAll();
    IEnumerable<MerchandiseCategory> GetAllCategories();
    IEnumerable<MerchandiseItem> GetAllMerchandise();
    ArticleItemForWarehouse GetById(ObjectId id);
    string Insert(ArticleItemForWarehouse entity);
    bool Update(ArticleItemForWarehouse entity);
}

public class ArticlesForWarehouseInLiteDbService : IArticlesForWarehouseInLiteDbService
{
    readonly ILiteCollection<ArticleItemForWarehouse> collection;

    public ArticlesForWarehouseInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Article_Warehouse")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<ArticleItemForWarehouse>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ArticleItemForWarehouse> GetAll() => collection.FindAll();

    public IEnumerable<MerchandiseItem> GetAllMerchandise() => collection.Find(Query.All())?.Select(x => x.Merchandise ?? new()) ?? [];

    public IEnumerable<MerchandiseCategory> GetAllCategories() => collection.Find(Query.All())?.Select(x => x.Merchandise?.Category ?? new())?.DistinctBy(c => c?.Name)?.Where(c => c != null) ?? [];

    public ArticleItemForWarehouse GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ArticleItemForWarehouse entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ArticleItemForWarehouse entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
