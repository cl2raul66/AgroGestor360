using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IWarehouseForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<WarehouseItem> GetAll();
    IEnumerable<MerchandiseCategory> GetAllCategories();
    IEnumerable<MerchandiseItem> GetAllMerchandise();
    WarehouseItem GetById(ObjectId id);
    string Insert(WarehouseItem entity);
    bool Update(WarehouseItem entity);
}

public class WarehouseForLitedbService : IWarehouseForLitedbService
{
    readonly ILiteCollection<WarehouseItem> collection;

    public WarehouseForLitedbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Warehouse")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<WarehouseItem>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<WarehouseItem> GetAll() => collection.FindAll();

    public IEnumerable<MerchandiseItem> GetAllMerchandise() => collection.Find(Query.All())?.Select(x => x.Merchandise ?? new()) ?? [];

    public IEnumerable<MerchandiseCategory> GetAllCategories() => collection.Find(Query.All())?.Select(x => x.Merchandise?.Category ?? new()) ?? [];

    public WarehouseItem GetById(ObjectId id) => collection.FindById(id);

    public string Insert(WarehouseItem entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(WarehouseItem entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
