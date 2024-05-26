﻿using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IQuotesInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(Guid code); 
    bool DeleteMany(IEnumerable<Guid> codes);
    IEnumerable<Quotation> GetAll();
    Quotation GetByCode(Guid code);
    IEnumerable<Quotation> GetExpiringQuotes(int expiryDays);
    string Insert(Quotation entity);
    void Rollback();
    bool Update(Quotation entity);
}

public class QuotesInLiteDbService : IQuotesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Quotation> collection;

    public QuotesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Quotes")
        };
        var mapper = new BsonMapper();

        mapper.Entity<Quotation>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<Quotation>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public Quotation GetByCode(Guid code) => collection.FindById(code);

    public IEnumerable<Quotation> GetAll() => collection.FindAll();

    public string Insert(Quotation entity) => collection.Insert(entity).AsGuid.ToString();

    public IEnumerable<Quotation> GetExpiringQuotes(int expiryDays)
    {
        var expiryDate = DateTime.Today.AddDays(-expiryDays);
        return collection.Find(q => q.Date <= expiryDate);
    }

    public bool Update(Quotation entity) => collection.Update(entity);

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
