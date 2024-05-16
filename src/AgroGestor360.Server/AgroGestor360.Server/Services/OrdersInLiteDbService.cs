using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IOrdersInLiteDbService
{
    bool Exist { get; }

    bool Delete(Guid code);
    IEnumerable<Order> GetAll();
    Order GetById(Guid code);
    string Insert(Order entity);
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

    public bool Exist => collection.Count() > 0;

    public Order GetById(Guid code) => collection.FindById(code);

    public IEnumerable<Order> GetAll() => collection.FindAll();

    public string Insert(Order entity) => collection.Insert(entity).AsGuid.ToString();

    public bool Update(Order entity) => collection.Update(entity);

    public bool Delete(Guid code) => collection.Delete(code);
}
