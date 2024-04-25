using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Services;

public interface ICustomersInLiteDbService
{
    bool Exist { get; }

    IEnumerable<Customer> GetAllWithDiscount();
    IEnumerable<Customer> GetAllWithoutDiscount();
    Customer GetById(ObjectId id);
    CustomerDiscountClass? GetDiscountById(int discountId);
    string Insert(Customer entity);
    bool Update(Customer entity);
}

public class CustomersInLiteDbService : ICustomersInLiteDbService
{
    readonly ILiteCollection<Customer> collection;

    public CustomersInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Customers")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<Customer>();
        collection.EnsureIndex(x => x.Discount!.Id);
    }

    public bool Exist => collection.Count() > 0;

    public Customer GetById(ObjectId id) => collection.FindById(id);

    public string Insert(Customer entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(Customer entity) => collection.Update(entity);

    public IEnumerable<Customer> GetAllWithDiscount() => collection.Find(Query.All()).Where(x => x.Discount is not null);

    public IEnumerable<Customer> GetAllWithoutDiscount() => collection.Find(Query.All()).Where(x => x.Discount is null);

    public CustomerDiscountClass? GetDiscountById(int discountId) => collection.FindOne(x => x.Discount != null && x.Discount.Id == discountId)?.Discount;
}
