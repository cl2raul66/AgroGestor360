﻿using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IProductsForSalesInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(ObjectId id);
    IEnumerable<ProductItemForSale> GetAll();
    IEnumerable<ProductItemForSale> GetAllByMerchandiseId(ObjectId merchandiseId);
    ProductItemForSale GetById(ObjectId id);
    string Insert(ProductItemForSale entity);
    void Rollback();
    bool Update(ProductItemForSale entity);
    int DeleteByMerchandiseId(ObjectId merchandiseId);
}

public class ProductsForSalesInLiteDbService : IProductsForSalesInLiteDbService
{
    readonly ILiteCollection<ProductItemForSale> collection;
    readonly LiteDatabase db;

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

    public bool Exist => collection.Count() > 0;

    public IEnumerable<ProductItemForSale> GetAll() => collection.FindAll();

    public IEnumerable<ProductItemForSale> GetAllByMerchandiseId(ObjectId merchandiseId) => collection.Find(x => x.MerchandiseId == merchandiseId);

    public ProductItemForSale GetById(ObjectId id) => collection.FindById(id);

    public string Insert(ProductItemForSale entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(ProductItemForSale entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);

    public int DeleteByMerchandiseId(ObjectId merchandiseId) => collection.DeleteMany(x => x.MerchandiseId == merchandiseId);
}
