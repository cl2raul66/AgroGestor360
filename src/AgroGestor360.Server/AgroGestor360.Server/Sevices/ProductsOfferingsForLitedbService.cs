using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IProductsOfferingsForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<ProductOffering> GetAll();
    ProductOffering GetById(ObjectId id);
    string Insert(ProductOffering entity);
}

public class ProductsOfferingsForLitedbService : IProductsOfferingsForLitedbService
{
    readonly ILiteCollection<ProductOffering> collection;

    public ProductsOfferingsForLitedbService(ProductsDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<ProductOffering>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ProductOffering> GetAll() => collection.FindAll();

    public ProductOffering GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ProductOffering entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Delete(ObjectId id) => collection.Delete(id);
}
