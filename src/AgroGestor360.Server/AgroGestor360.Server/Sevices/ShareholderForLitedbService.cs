using AgroGestor360.Server.Tools.Helpers;
using LiteDB;
using vCardLib.Models;

namespace AgroGestor360.Server.Sevices;

public interface IShareholderForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<vCard> GetAllById(ObjectId id);
    string Insert(vCard vCard);
    bool Update(vCard vCard);
}

public class ShareholderForLitedbService : IShareholderForLitedbService
{
    readonly ILiteCollection<vCard> collection;

    public ShareholderForLitedbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("Shareholder")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<vCard>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<vCard> GetAllById(ObjectId id) => collection.Find(x => x.Uid == id.ToString());

    public string Insert(vCard vCard)
    {
        try
        {
            return collection.Insert(vCard);
        }
        catch (Exception)
        {

            return string.Empty;
        }
    }

    public bool Update(vCard vCard) => collection.Update(vCard);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
