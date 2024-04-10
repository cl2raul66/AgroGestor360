using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IMerchandiseForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<Merchandise> GetAllByName(string name);
    IEnumerable<ObjectId> GetAllCategories();
    IEnumerable<Merchandise> GetAllDisabled();
    IEnumerable<Merchandise> GetAllEnabled();
    Merchandise GetById(ObjectId id);
    string Insert(Merchandise entity);
    bool Update(Merchandise entity);
}

public class MerchandiseForLitedbService : IMerchandiseForLitedbService
{
    readonly ILiteCollection<Merchandise> collection;

    public MerchandiseForLitedbService(MerchandiseDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<Merchandise>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<Merchandise> GetAllEnabled() => collection.Find(x => !x.Disabled);

    public IEnumerable<Merchandise> GetAllDisabled() => collection.Find(x => x.Disabled);

    public IEnumerable<ObjectId> GetAllCategories() => collection.FindAll().Select(x => x.MerchandiseCategoryId!).Distinct();

    public Merchandise GetById(ObjectId id) => collection.FindById(id);

    public IEnumerable<Merchandise> GetAllByName(string name) => collection.Find(x => x.Name!.Contains(name));

    public string Insert(Merchandise entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(Merchandise entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
