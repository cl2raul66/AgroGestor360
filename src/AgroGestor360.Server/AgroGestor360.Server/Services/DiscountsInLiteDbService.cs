using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IDiscountsInLiteDbService : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(int id);
    bool ExistById(int id);
    IEnumerable<DiscountForCustomer> GetAll();
    DiscountForCustomer GetById(int id);
    int Insert(DiscountForCustomer entity);
    void Rollback();
    bool Update(DiscountForCustomer entity);
}

public class DiscountsInLiteDbService : IDiscountsInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<DiscountForCustomer> collection;

    public DiscountsInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Discount")
        };

        db = new(cnx);
        collection = db.GetCollection<DiscountForCustomer>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();


    public bool Exist => collection.Count() > 0;

    public bool ExistById(int id) => collection.FindById(id) is not null;

    public DiscountForCustomer GetById(int id) => collection.FindById(id);

    public int Insert(DiscountForCustomer entity) => collection.Insert(entity).AsInt32;

    public bool Update(DiscountForCustomer entity) => collection.Update(entity);

    public IEnumerable<DiscountForCustomer> GetAll() => collection.FindAll();

    public bool Delete(int id) => collection.Delete(id);
}
