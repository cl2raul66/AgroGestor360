using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IMerchandiseInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(ObjectId id);
    IEnumerable<MerchandiseItem> GetAll();
    IEnumerable<string> GetAllCategories();
    MerchandiseItem GetById(ObjectId id);
    string Insert(MerchandiseItem entity);
    void Rollback();
    bool Update(MerchandiseItem entity); 
    bool UpdateMany(IEnumerable<MerchandiseItem> entities);
}

public class MerchandiseInLiteDbService : IMerchandiseInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<MerchandiseItem> collection;

    public MerchandiseInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Merchandise")
        };

        db = new(cnx);
        collection = db.GetCollection<MerchandiseItem>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public IEnumerable<MerchandiseItem> GetAll() => collection.FindAll();

    public IEnumerable<string> GetAllCategories() => collection.Find(Query.All()).Select(x => x.Category ?? string.Empty).Distinct();

    public MerchandiseItem GetById(ObjectId id) => collection.FindById(id);

    public string Insert(MerchandiseItem entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(MerchandiseItem entity) => collection.Update(entity);

    public bool UpdateMany(IEnumerable<MerchandiseItem> entities)
    {
        try
        {
            db.BeginTrans();

            foreach (var entity in entities)
            {
                var result = collection.Update(entity);

                if (!result)
                {
                    db.Rollback();
                    return false;
                }
            }

            db.Commit();
            return true;
        }
        catch
        {
            db.Rollback();
            return false;
        }
    }

    public bool Delete(ObjectId id) => collection.Delete(id);
}
