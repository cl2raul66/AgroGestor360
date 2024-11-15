using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IProductsForSalesInLiteDbService : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(ObjectId id);
    int DeleteByMerchandiseId(ObjectId merchandiseId);
    IEnumerable<ProductItemForSale> GetAll();
    IEnumerable<ProductItemForSale> GetAllByMerchandiseId(ObjectId merchandiseId);
    ProductItemForSale GetById(ObjectId id);
    IEnumerable<ProductItemForSale> GetManyById(IEnumerable<ObjectId> ids);
    string Insert(ProductItemForSale entity);
    void Rollback();
    bool Update(ProductItemForSale entity);
}

public class ProductsForSalesInLiteDbService : IProductsForSalesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<ProductItemForSale> collection;

    public ProductsForSalesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Products_Sales")
        };

        db = new(cnx);
        collection = db.GetCollection<ProductItemForSale>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ProductItemForSale> GetAll() => collection.FindAll();

    public IEnumerable<ProductItemForSale> GetAllByMerchandiseId(ObjectId merchandiseId) => collection.Find(x => x.MerchandiseId == merchandiseId);

    public ProductItemForSale GetById(ObjectId id) => collection.FindById(id);

    public IEnumerable<ProductItemForSale> GetManyById(IEnumerable<ObjectId> ids) => collection.Find(x => ids.Contains(x.Id));

    public string Insert(ProductItemForSale entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ProductItemForSale entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);

    public int DeleteByMerchandiseId(ObjectId merchandiseId) => collection.DeleteMany(x => x.MerchandiseId == merchandiseId);
}
