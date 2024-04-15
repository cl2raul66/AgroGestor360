using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IProductsForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<ProductItemForSale> GetAll();
    ProductItemForSale GetById(ObjectId id);
    string Insert(ProductItemForSale entity);
    bool Update(ProductItemForSale entity);
}

public class ProductsForLitedbService : IProductsForLitedbService
{
    readonly ILiteCollection<ProductItemForSale> collection;

    public ProductsForLitedbService(ProductsDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<ProductItemForSale>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ProductItemForSale> GetAll() => collection.FindAll();

    public ProductItemForSale GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ProductItemForSale entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ProductItemForSale entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
