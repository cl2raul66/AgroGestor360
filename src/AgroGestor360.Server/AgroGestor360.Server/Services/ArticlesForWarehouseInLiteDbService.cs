using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IArticlesForWarehouseInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(ObjectId id);
    IEnumerable<ArticleItemForWarehouse> GetAll();
    ArticleItemForWarehouse GetById(ObjectId id);
    string Insert(ArticleItemForWarehouse entity);
    void Rollback();
    bool Update(ArticleItemForWarehouse entity);
    bool UpdateMany(ArticleItemForWarehouse[] entity);
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

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ArticleItemForWarehouse> GetAll() => collection.FindAll();

    public ArticleItemForWarehouse GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ArticleItemForWarehouse entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ArticleItemForWarehouse entity) => collection.Update(entity);

    public bool UpdateMany(ArticleItemForWarehouse[] entity)
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
        return changed.Count(x => x) == entity.Length;
    }

    public bool Delete(ObjectId id) => collection.Delete(id);
}
