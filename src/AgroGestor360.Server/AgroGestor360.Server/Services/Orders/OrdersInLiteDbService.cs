﻿using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IOrdersInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(Guid code);
    bool DeleteMany(IEnumerable<Guid> codes);
    IEnumerable<Order> GetAll();
    Order GetByCode(Guid code);
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

    public bool Exist => collection.Count() > 0;

    public IEnumerable<Order> GetAll() => collection.FindAll();

    public Order GetByCode(Guid code) => collection.FindById(code);

    public IEnumerable<Order> GetExpiringOrders(int expiryDays)
    {
        var expiryDate = DateTime.Today.AddDays(-expiryDays);
        return collection.Find(q => q.Date <= expiryDate);
    }

    public string Insert(Order entity) => collection.Insert(entity).AsGuid.ToString();

    public bool Update(Order entity) => collection.Update(entity);

    public bool Delete(Guid code) => collection.Delete(code);

    public bool DeleteMany(IEnumerable<Guid> codes)
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
