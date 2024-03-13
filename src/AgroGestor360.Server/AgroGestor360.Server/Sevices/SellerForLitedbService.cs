using AgroGestor360.Server.Tools.Helpers;
using LiteDB;
using vCardLib.Models;

namespace AgroGestor360.Server.Sevices;

public interface ISellerForLitedbService
{
    bool Delete(string id);
    IEnumerable<vCard> GetallByNameSection(string name);
    IEnumerable<vCard> GetallByPhoneSection(string number);
    IEnumerable<vCard> GetallLimit(int end, int begin = 0);
    vCard GetById(string id);
    bool Insert(vCard card);
    bool Update(vCard card);
}

public class SellerForLitedbService : ISellerForLitedbService
{
    readonly ILiteCollection<vCard> collection;

    public SellerForLitedbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Seller")
        };

        var mapper = new BsonMapper();

        mapper.Entity<vCard>()
            .Id(vcard => vcard.Uid);

        LiteDatabase db = new(cnx, mapper);

        collection = db.GetCollection<vCard>();
    }

    public IEnumerable<vCard> GetallLimit(int end, int begin = 0)
    {
        int t = collection.Count();
        if (end > t)
        {
            end = t;
        }
        if (begin == 0)
        {
            begin = t - end;
        }
        else
        {
            begin = t - begin - end;
        }
        if (begin < 0 || begin > t)
        {
            begin = 0;
        }
        return collection.Find(Query.All(), skip: begin, limit: end).Reverse();
    }

    public IEnumerable<vCard> GetallByNameSection(string name) => collection.Find(x => x.FormattedName!.Contains(name));

    public IEnumerable<vCard> GetallByPhoneSection(string number) => collection.Find(x => x.PhoneNumbers.Any(n => n.Number.Contains(number)));

    public vCard GetById(string id) => collection.FindById(id);

    public bool Insert(vCard card)
    {
        card.Uid = ObjectId.NewObjectId().ToString();
        return collection.Insert(card) is not null;
    }

    public bool Update(vCard card) => collection.Update(card);

    public bool Delete(string id) => collection.Delete(id);
}
