using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface IWasteInvoicesInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    IEnumerable<WasteInvoice> GetAll();
    WasteInvoice GetById(string code);
    IEnumerable<WasteInvoice> GetInvoicesByDateRange(DateTime endDate, DateTime? startDate, InvoiceStatus? status, ObjectId? sellerId, ObjectId? customerId);
    string Insert(WasteInvoice entity);
    void Rollback();
    bool Update(WasteInvoice entity);
}

public class WasteInvoicesInLiteDbService : IWasteInvoicesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<WasteInvoice> collection;

    public WasteInvoicesInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Invoices_Waste")
        };
        var mapper = new BsonMapper();

        mapper.Entity<WasteInvoice>().Id(x => x.Code);

        db = new(cnx, mapper);

        collection = db.GetCollection<WasteInvoice>();
        collection.EnsureIndex(x => x.Code);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public WasteInvoice GetById(string code) => collection.FindById(code);

    public IEnumerable<WasteInvoice> GetAll() => collection.FindAll();

    //public IEnumerable<WasteInvoice> GetInvoicesByDateRange(DateTime endDate, DateTime? beginDate, InvoiceStatus? status, ObjectId? sellerId, ObjectId? customerId) => beginDate.HasValue
    //    ? collection.Find(x => x.Date > beginDate!.Value.AddDays(-1) && x.Date < endDate.AddDays(1)).Where(invoice => (status is null || invoice.Status == status) && (sellerId is null || invoice.Seller?.Id == sellerId) && (customerId is null || invoice.Customer?.Id == customerId))
    //    : collection.Find(x => x.Date < endDate.AddDays(1)).Where(invoice => (status is null || invoice.Status == status) && (sellerId is null || invoice.Seller?.Id == sellerId) && (customerId is null || invoice.Customer?.Id == customerId));

    public IEnumerable<WasteInvoice> GetInvoicesByDateRange(DateTime endDate, DateTime? beginDate, InvoiceStatus? status, ObjectId? sellerId, ObjectId? customerId)
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

    public string Insert(WasteInvoice entity) => collection.Insert(entity).AsString;

    public bool Update(WasteInvoice entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);
}
