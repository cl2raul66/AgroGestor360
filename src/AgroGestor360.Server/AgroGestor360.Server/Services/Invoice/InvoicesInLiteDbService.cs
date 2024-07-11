using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Enums;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;
using System.Collections;

namespace AgroGestor360.Server.Services;

public interface IInvoicesInLiteDbService
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(string code);
    bool DeleteConcept(int id);
    IEnumerable<Invoice> GetAll();
    Invoice GetByCode(string code);
    ConceptForDeletedInvoice? GetConceptByNote(string note);
    IEnumerable<ConceptForDeletedInvoice> GetConcepts();
    IEnumerable<Invoice> GetInvoicesByDateRange(DateTime endDate, DateTime? startDate, InvoiceStatus? status, ObjectId? sellerId, ObjectId? customerId);
    IEnumerable<Invoice> GetManyByCodes(IEnumerable<string> codes);
    string Insert(Invoice entity);
    int InsertConcept(ConceptForDeletedInvoice entity);
    bool InsertMany(IEnumerable<Invoice> entities);
    void Rollback();
    bool Update(Invoice entity);
}

public class InvoicesInLiteDbService : IInvoicesInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Invoice> collection;
    readonly ILiteCollection<ConceptForDeletedInvoice> collection1;

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

        collection1 = db.GetCollection<ConceptForDeletedInvoice>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public bool Exist => collection.Count() > 0;

    public Invoice GetByCode(string code) => collection.FindById(code);

    public IEnumerable<Invoice> GetManyByCodes(IEnumerable<string> codes) => codes.Any()
        ? collection.Find(x => codes.Contains(x.Code))
        : [];

    public IEnumerable<Invoice> GetAll() => collection.FindAll();

    //public IEnumerable<Invoice> GetInvoicesByDateRange(DateTime endDate, DateTime? beginDate, InvoiceStatus? status, ObjectId? sellerId, ObjectId? customerId) => beginDate.HasValue 
    //    ? collection.Find(x => x.Date < endDate.AddDays(1)).Where(invoice => (status is null || invoice.Status == status) &&
    //(sellerId is null || invoice.Seller?.Id == sellerId) && (customerId is null || invoice.Customer?.Id == customerId))
    //    : collection.Find(x => x.Date > beginDate!.Value.AddDays(-1) && x.Date < endDate.AddDays(1)).Where(invoice => (status is null || invoice.Status == status) && (sellerId is null || invoice.Seller?.Id == sellerId) && (customerId is null || invoice.Customer?.Id == customerId));
    public IEnumerable<Invoice> GetInvoicesByDateRange(DateTime endDate, DateTime? beginDate, InvoiceStatus? status, ObjectId? sellerId, ObjectId? customerId)
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

    public string Insert(Invoice entity) => collection.Insert(entity).AsString;

    public bool InsertMany(IEnumerable<Invoice> entities)
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

    public bool Update(Invoice entity) => collection.Update(entity);

    public bool Delete(string code) => collection.Delete(code);


    public IEnumerable<ConceptForDeletedInvoice> GetConcepts() => collection1.FindAll();

    public ConceptForDeletedInvoice? GetConceptByNote(string note) => collection1.FindOne(x => x.Concept == note);

    public int InsertConcept(ConceptForDeletedInvoice entity) => collection1.Insert(entity).AsInt32;

    public bool DeleteConcept(int id) => collection1.Delete(id);
}
