using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IMerchandiseCategoryForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<MerchandiseCategory> GetAllDisabled();
    IEnumerable<MerchandiseCategory> GetAllEnabled();
    MerchandiseCategory GetById(ObjectId id);
    string Insert(MerchandiseCategory entity);
    bool Update(MerchandiseCategory entity);
}

public class MerchandiseCategoryForLitedbService : IMerchandiseCategoryForLitedbService
{
    readonly ILiteCollection<MerchandiseCategory> collection;

    public MerchandiseCategoryForLitedbService(MerchandiseDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<MerchandiseCategory>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<MerchandiseCategory> GetAllEnabled() => collection.Find(x => !x.Disabled);

    public IEnumerable<MerchandiseCategory> GetAllDisabled() => collection.Find(x => x.Disabled);

    public MerchandiseCategory GetById(ObjectId id) => collection.FindById(id);

    public string Insert(MerchandiseCategory entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(MerchandiseCategory entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
