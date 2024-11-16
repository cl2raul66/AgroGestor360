using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface IUserForLitedbService :IDisposable
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<User> GetAllById(ObjectId id);
    IEnumerable<User> GetAllByIsAuthorized();
    IEnumerable<User> GetAllByIsNotAuthorized();
    string Insert(User User);
    bool Update(User User);
}

public class UserForLiteDbService : IUserForLitedbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<User> collection;

    public UserForLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("User")
        };

        db = new(cnx);
        collection = db.GetCollection<User>();
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();

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
