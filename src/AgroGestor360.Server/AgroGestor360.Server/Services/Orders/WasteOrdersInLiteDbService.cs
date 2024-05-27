using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IWasteOrdersInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    IEnumerable<Order> GetAll();
    Order GetByCode(string code);
    string Insert(Order entity);
    bool InsertMany(IEnumerable<Order> entities);
    void Rollback();
    bool Update(Order entity);
}

public class WasteOrdersInLiteDbService : IWasteOrdersInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Order> collection;

    public WasteOrdersInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Orders_Waste")
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

    public bool Exist => collection.Count() > 0;

    public Order GetByCode(string code) => collection.FindById(code);

    public IEnumerable<Order> GetAll() => collection.FindAll();

    public string Insert(Order entity) => collection.Insert(entity).AsString;

    public bool InsertMany(IEnumerable<Order> entities) => collection.InsertBulk(entities) == entities.Count();

    public bool Update(Order entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);
}
