using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface ISellersInLiteDbService:IDisposable
{
    bool Exist { get; }
    bool ExistById(ObjectId id);

    bool Delete(ObjectId id);
    IEnumerable<Seller> GetAll();
    Seller GetById(ObjectId id);
    string Insert(Seller entity);
    bool Update(Seller entity);
}

public class SellersInLiteDbService : ISellersInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Seller> collection;

    public SellersInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Sellers")
        };

        db = new(cnx);
        collection = db.GetCollection<Seller>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

    public bool Exist => collection.Count() > 0;

    public bool ExistById(ObjectId id) => collection.FindById(id) is not null;

    public IEnumerable<Seller> GetAll() => collection.FindAll();

    public Seller GetById(ObjectId id) => collection.FindById(id);

    public string Insert(Seller entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(Seller entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
