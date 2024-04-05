using AgroGestor360.Server.Tools.Configurations;
using LiteDB;
using vCardLib.Models;

namespace AgroGestor360.Server.Sevices;

public interface ICustomersForLitedbService
{
    bool Exist { get; }

    bool Delete(string id);
    IEnumerable<vCard> GetAll();
    IEnumerable<vCard> GetAllByName(string name);
    vCard GetById(string id);
    string Insert(vCard entity);
    bool Update(vCard entity);
}

public class CustomersForLitedbService : ICustomersForLitedbService
{
    readonly ILiteCollection<vCard> collection;

    public CustomersForLitedbService(ContactsDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<vCard>("Customer");
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<vCard> GetAll() => collection.FindAll();

    public IEnumerable<vCard> GetAllByName(string name) => collection.Find(x => x.FormattedName!.Contains(name));

    public vCard GetById(string id) => collection.FindById(id);

    public string Insert(vCard entity) => collection.Insert(entity).ToString();

    public bool Update(vCard entity) => collection.Update(entity);

    public bool Delete(string id) => collection.Delete(id);
}
