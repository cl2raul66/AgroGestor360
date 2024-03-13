using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IUserForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<User> GetAllById(ObjectId id);
    IEnumerable<User> GetAllByIsAuthorized();
    IEnumerable<User> GetAllByIsNotAuthorized();
    string Insert(User User);
    bool Update(User User);
}

public class UserForLitedbService : IUserForLitedbService
{
    readonly ILiteCollection<User> collection;

    public UserForLitedbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("User")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<User>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<User> GetAllByIsAuthorized() => collection.Find(x => x.IsAuthorized).Reverse();

    public IEnumerable<User> GetAllByIsNotAuthorized() => collection.Find(x => !x.IsAuthorized).Reverse();

    public IEnumerable<User> GetAllById(ObjectId id) => collection.Find(x => x.Id == id);


    public string Insert(User User)
    {
        try
        {
            return collection.Insert(User);
        }
        catch (Exception)
        {

            return string.Empty;
        }
    }

    public bool Update(User User) => collection.Update(User);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
