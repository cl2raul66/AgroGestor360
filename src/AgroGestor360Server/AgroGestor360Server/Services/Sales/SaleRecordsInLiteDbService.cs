﻿using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Enums;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface ISaleRecordsInLiteDbService : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    bool DeleteConcept(int id);
    IEnumerable<SaleRecord> GetAll();
    SaleRecord GetByCode(string code);
    ConceptForDeletedSaleRecord? GetConceptByNote(string note);
    IEnumerable<ConceptForDeletedSaleRecord> GetConcepts();
    IEnumerable<SaleRecord> GetSaleRecordsByDateRange(DateTime endDate, DateTime? startDate, SaleStatus? status, ObjectId? sellerId, ObjectId? customerId);
    IEnumerable<SaleRecord> GetManyByCodes(IEnumerable<string> codes);
    string Insert(SaleRecord entity);
    int InsertConcept(ConceptForDeletedSaleRecord entity);
    bool InsertMany(IEnumerable<SaleRecord> entities);
    void Rollback();
    bool Update(SaleRecord entity);
}

public class SaleRecordsInLiteDbService : ISaleRecordsInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<SaleRecord> collection;
    readonly ILiteCollection<ConceptForDeletedSaleRecord> collection1;

    public SaleRecordsInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("SaleRecords")
        };
        var mapper = new BsonMapper();

        mapper.Entity<SaleRecord>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<SaleRecord>();
        collection.EnsureIndex(x => x.Code);

        collection1 = db.GetCollection<ConceptForDeletedSaleRecord>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

    public bool Exist => collection.Count() > 0;

    public SaleRecord GetByCode(string code) => collection.FindById(code);

    public IEnumerable<SaleRecord> GetManyByCodes(IEnumerable<string> codes) => codes.Any()
        ? collection.Find(x => codes.Contains(x.Code))
        : [];

    public IEnumerable<SaleRecord> GetAll() => collection.FindAll();

    public IEnumerable<SaleRecord> GetSaleRecordsByDateRange(DateTime endDate, DateTime? beginDate, SaleStatus? status, ObjectId? sellerId, ObjectId? customerId)
    {
        var query = collection.Query();

        query = query.Where(x => x.Date.Date <= endDate.Date);

        if (beginDate.HasValue)
        {
            query = query.Where(x => x.Date.Date >= beginDate.Value.Date);
        }

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        if (sellerId is not null)
        {
            query = query.Where(x => x.Seller!.Id == sellerId);
        }

        if (customerId is not null)
        {
            query = query.Where(x => x.Customer!.Id == customerId);
        }

        return query.ToEnumerable();
    }

    public string Insert(SaleRecord entity) => collection.Insert(entity).AsString;

    public bool InsertMany(IEnumerable<SaleRecord> entities)
    {
        try
        {
            db.BeginTrans();

            foreach (var entity in entities)
            {
                collection.Insert(entity);
            }

            db.Commit();
            return true;
        }
        catch
        {
            db.Rollback();
            return false;
        }
    }

    public bool Update(SaleRecord entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);


    public IEnumerable<ConceptForDeletedSaleRecord> GetConcepts() => collection1.FindAll();

    public ConceptForDeletedSaleRecord? GetConceptByNote(string note) => collection1.FindOne(x => x.Concept == note);

    public int InsertConcept(ConceptForDeletedSaleRecord entity) => collection1.Insert(entity).AsInt32;

    public bool DeleteConcept(int id) => collection1.Delete(id);
}
