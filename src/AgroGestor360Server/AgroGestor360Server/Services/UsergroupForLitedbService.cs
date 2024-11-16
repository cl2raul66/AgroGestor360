using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public class UsergroupForLitedbService
{
    readonly ILiteCollection<UserGroup> collection;

    public UsergroupForLitedbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("UserGroup")
        };

        LiteDatabase db = new(cnx);
        collection = db.GetCollection<UserGroup>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<UserGroup> GetAllById(string id) => collection.Find(x => x.Id == id);

    public string Insert(UserGroup UserGroup)
    {
        try
        {
            return collection.Insert(UserGroup);
        }
        catch (Exception)
        {

            return string.Empty;
        }
    }

    public bool Delete(string id) => collection.Delete(id);
}
