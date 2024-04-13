using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IProductsForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<Product> GetAll();
    Product GetById(ObjectId id);
    string Insert(Product entity);
    bool Update(Product entity);
}

public class ProductsForLitedbService : IProductsForLitedbService
{
    readonly ILiteCollection<Product> collection;

    public ProductsForLitedbService(ProductsDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<Product>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<Product> GetAll() => collection.FindAll();

    public Product GetById(ObjectId id) => collection.FindById(id);

    public string Insert(Product entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(Product entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
