using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IWasteSaleRecordsInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    IEnumerable<WasteSaleRecord> GetAll();
    WasteSaleRecord GetById(string code);
    IEnumerable<WasteSaleRecord> GetWasteSaleRecordsByDateRange(DateTime endDate, DateTime? startDate, SaleStatus? status, ObjectId? sellerId, ObjectId? customerId);
    string Insert(WasteSaleRecord entity);
    void Rollback();
    bool Update(WasteSaleRecord entity);
}

public class WasteSaleRecordsInLiteDbService : IWasteSaleRecordsInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<WasteSaleRecord> collection;

    public WasteSaleRecordsInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Invoices_Waste")
        };
        var mapper = new BsonMapper();

        mapper.Entity<WasteSaleRecord>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<WasteSaleRecord>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public WasteSaleRecord GetById(string code) => collection.FindById(code);

    public IEnumerable<WasteSaleRecord> GetAll() => collection.FindAll();

    public IEnumerable<WasteSaleRecord> GetWasteSaleRecordsByDateRange(DateTime endDate, DateTime? beginDate, SaleStatus? status, ObjectId? sellerId, ObjectId? customerId)
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

    public string Insert(WasteSaleRecord entity) => collection.Insert(entity).AsString;

    public bool Update(WasteSaleRecord entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);
}
