using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface ITypesDiscountsCustomersService
{
    bool Exist { get; }

    bool Delete(int id);
    IEnumerable<CustomerDiscountClass> GetAll();
    CustomerDiscountClass GetById(int id);
    int Insert(CustomerDiscountClass entity);
    bool Update(CustomerDiscountClass entity);
}

public class TypesDiscountsCustomersService : ITypesDiscountsCustomersService
{
    readonly ILiteCollection<CustomerDiscountClass> collection;

    public TypesDiscountsCustomersService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("CustomerClass_Discount")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<CustomerDiscountClass>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<CustomerDiscountClass> GetAll() => collection.FindAll();

    public CustomerDiscountClass GetById(int id) => collection.FindById(id);

    public int Insert(CustomerDiscountClass entity) => collection.Insert(entity)?.AsInt32 ?? 0;

    public bool Update(CustomerDiscountClass entity) => collection.Update(entity);

    public bool Delete(int id) => collection.Delete(id);
}
