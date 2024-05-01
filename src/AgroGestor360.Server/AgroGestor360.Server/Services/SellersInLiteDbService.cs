using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface ISellersInLiteDbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<Seller> GetAll();
    Seller GetById(ObjectId id);
    string Insert(Seller entity);
    bool Update(Seller entity);
}

public class SellersInLiteDbService : ISellersInLiteDbService
{
    readonly ILiteCollection<Seller> collection;

    public SellersInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Sellers")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<Seller>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<Seller> GetAll() => collection.FindAll();

    public Seller GetById(ObjectId id) => collection.FindById(id);

    public string Insert(Seller entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(Seller entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
