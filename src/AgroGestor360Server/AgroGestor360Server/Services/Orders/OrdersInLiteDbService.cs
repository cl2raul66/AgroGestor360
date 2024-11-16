using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface IOrdersInLiteDbService : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    bool DeleteMany(IEnumerable<string> codes);
    IEnumerable<Order> GetAll();
    Order GetByCode(string code);
    IEnumerable<Order> GetExpiringOrders(int expiryDays);
    string Insert(Order entity);
    void Rollback();
    bool Update(Order entity);
}

public class OrdersInLiteDbService : IOrdersInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Order> collection;

    public OrdersInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Orders")
        };
        var mapper = new BsonMapper();

        mapper.Entity<Order>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<Order>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();


    public bool Exist => collection.Count() > 0;

    public IEnumerable<Order> GetAll() => collection.FindAll();

    public Order GetByCode(string code) => collection.FindById(code);

    public IEnumerable<Order> GetExpiringOrders(int expiryDays)
    {
        var expiryDate = DateTime.Today.AddDays(-expiryDays);
        return collection.Find(q => q.Date <= expiryDate);
    }

    public string Insert(Order entity) => collection.Insert(entity).AsString;

    public bool Update(Order entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);

    public bool DeleteMany(IEnumerable<string> codes)
    {
        var count = 0;
        foreach (var code in codes)
        {
            if (!collection.Delete(code))
            {
                return false;
            }
            count++;
        }
        return count == codes.Count();
    }
}
