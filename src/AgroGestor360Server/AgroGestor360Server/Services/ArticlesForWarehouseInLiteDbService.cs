using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface IArticlesForWarehouseInLiteDbService :IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(ObjectId id);
    IEnumerable<ArticleItemForWarehouse> GetAll();
    ArticleItemForWarehouse GetById(ObjectId id);
    IEnumerable<ArticleItemForWarehouse> GetManyByIds(IEnumerable<ObjectId> ids);
    string Insert(ArticleItemForWarehouse entity);
    void Rollback();
    bool Update(ArticleItemForWarehouse entity);
    bool UpdateMany(IEnumerable<ArticleItemForWarehouse> entity);
}

public class ArticlesForWarehouseInLiteDbService : IArticlesForWarehouseInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<ArticleItemForWarehouse> collection;

    public ArticlesForWarehouseInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Article_Warehouse")
        };

        var mapper = new BsonMapper();
        mapper.Entity<ArticleItemForWarehouse>().Id(x => x.MerchandiseId);

        db = new(cnx, mapper);
        collection = db.GetCollection<ArticleItemForWarehouse>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ArticleItemForWarehouse> GetAll() => collection.FindAll();

    public ArticleItemForWarehouse GetById(ObjectId id) => collection.FindById(id);

    public IEnumerable<ArticleItemForWarehouse> GetManyByIds(IEnumerable<ObjectId> ids) => collection.Find(x => ids.Contains(x.MerchandiseId));

    public string Insert(ArticleItemForWarehouse entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ArticleItemForWarehouse entity) => collection.Update(entity);

    public bool UpdateMany(IEnumerable<ArticleItemForWarehouse> entity)
    {
        db.BeginTrans();
        List<bool> changed = [];
        foreach (var item in entity)
        {
            try
            {
                changed.Add(collection.Update(item));
            }
            catch (Exception)
            {
                db.Rollback();
                return false;
            }
        }
        db.Commit();
        return changed.Count(x => x) == entity.Count();
    }

    public bool Delete(ObjectId id) => collection.Delete(id);
}
