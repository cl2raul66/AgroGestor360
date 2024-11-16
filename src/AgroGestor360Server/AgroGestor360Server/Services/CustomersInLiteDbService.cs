using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface ICustomersInLiteDbService : IDisposable
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    bool ExistById(ObjectId id);
    IEnumerable<Customer> GetAll();
    IEnumerable<DiscountForCustomer> GetAllDiscount();
    IEnumerable<Customer> GetAllWithDiscount();
    IEnumerable<Customer> GetAllWithoutDiscount();
    Customer GetById(ObjectId id);
    DiscountForCustomer? GetDiscountById(int discountId);
    string Insert(Customer entity);
    bool Update(Customer entity);
}

public class CustomersInLiteDbService : ICustomersInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<Customer> collection;

    public CustomersInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Customers")
        };

        db = new(cnx);
        collection = db.GetCollection<Customer>();
        collection.EnsureIndex(x => x.Discount!.Id);
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

    public bool Exist => collection.Count() > 0;

    public bool ExistById(ObjectId id) => collection.FindById(id) is not null;

    public Customer GetById(ObjectId id) => collection.FindById(id);

    public string Insert(Customer entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(Customer entity) => collection.Update(entity);

    public IEnumerable<Customer> GetAll() => collection.FindAll();

    public IEnumerable<Customer> GetAllWithDiscount() => collection.Find(Query.All()).Where(x => x.Discount is not null);

    public IEnumerable<Customer> GetAllWithoutDiscount() => collection.Find(Query.All()).Where(x => x.Discount is null);

    public IEnumerable<DiscountForCustomer> GetAllDiscount() => collection.Find(x => x.Discount != null).Select(x => x.Discount!);

    public DiscountForCustomer? GetDiscountById(int discountId) => collection.FindOne(x => x.Discount != null && x.Discount.Id == discountId)?.Discount;

    public bool Delete(ObjectId id) => collection.Delete(id);
}
