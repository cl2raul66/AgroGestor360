using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IWarehouseForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<WarehouseItem> GetAll();
    WarehouseItem GetById(ObjectId id);
    WarehouseItem GetByMerchandiseId(ObjectId merchandiseId);
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

    public WarehouseItem GetById(ObjectId id) => collection.FindById(id);

    public WarehouseItem GetByMerchandiseId(ObjectId merchandiseId) => collection.FindOne(x => x.MerchandiseId!.ToString() == merchandiseId.ToString());

    public string Insert(WarehouseItem entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(WarehouseItem entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
