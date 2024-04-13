using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IMerchandiseForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<MerchandiseItem> GetAll();
    IEnumerable<MerchandiseCategory> GetAllCategories();
    MerchandiseItem GetById(ObjectId id);
    string Insert(MerchandiseItem entity);
    bool Update(MerchandiseItem entity);
}

public class MerchandiseForLitedbService : IMerchandiseForLitedbService
{
    readonly ILiteCollection<MerchandiseItem> collection;

    public MerchandiseForLitedbService(MerchandiseDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<MerchandiseItem>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<MerchandiseItem> GetAll() => collection.FindAll() ?? [];

    public IEnumerable<MerchandiseCategory> GetAllCategories() => collection.Find(Query.All()).Select(x => x.Category ?? new())?.Distinct() ?? [];

    public MerchandiseItem GetById(ObjectId id) => collection.FindById(id);

    public string Insert(MerchandiseItem entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(MerchandiseItem entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
