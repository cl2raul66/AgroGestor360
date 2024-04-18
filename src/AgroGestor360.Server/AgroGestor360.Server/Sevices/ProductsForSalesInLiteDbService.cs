using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IProductsForSalesInLiteDbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<ProductItemForSale> GetAll();
    ProductItemForSale GetById(ObjectId id);
    string Insert(ProductItemForSale entity);
    bool Update(ProductItemForSale entity);
}

public class ProductsForSalesInLiteDbService : IProductsForSalesInLiteDbService
{
    readonly ILiteCollection<ProductItemForSale> collection;

    public ProductsForSalesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Products_Sales")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<ProductItemForSale>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ProductItemForSale> GetAll() => collection.FindAll();

    public ProductItemForSale GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ProductItemForSale entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ProductItemForSale entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
