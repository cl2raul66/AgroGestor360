﻿using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IInvoicesInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(Guid code);
    IEnumerable<Invoice> GetAll();
    Invoice GetByCode(Guid code);
    IEnumerable<Invoice> GetManyByCodes(IEnumerable<Guid> codes);
    string Insert(Invoice entity);
    void Rollback();
    bool Update(Invoice entity);
}

public class InvoicesInLiteDbService : IInvoicesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Invoice> collection;

    public InvoicesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Invoices")
        };
        var mapper = new BsonMapper();

        mapper.Entity<Invoice>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<Invoice>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public Invoice GetByCode(Guid code) => collection.FindById(code);

    public IEnumerable<Invoice> GetManyByCodes(IEnumerable<Guid> codes) => codes.Any() 
        ? collection.Find(x => codes.Contains(x.Code)) 
        : [];

    public IEnumerable<Invoice> GetAll() => collection.FindAll();

    public string Insert(Invoice entity) => collection.Insert(entity).AsGuid.ToString();

    public bool Update(Invoice entity) => collection.Update(entity);

    public bool Delete(Guid code) => collection.Delete(code);
}