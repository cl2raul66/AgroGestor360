using AgroGestor360Server.Models;
using AgroGestor360Server.Tools.Helpers;
using LiteDB;

namespace AgroGestor360Server.Services;

public interface ILineCreditsInLiteDbService : IDisposable
{
    bool Exist { get; }

    void BeginTrans();
    void Commit();
    bool Delete(int id, bool withInternalTrans = true);
    bool Delete1(int id, bool withInternalTrans = true);
    bool ExistById(int id);
    IEnumerable<LineCredit> GetAll();
    IEnumerable<TimeLimitForCredit> GetAll1();
    LineCredit GetById(int id);
    TimeLimitForCredit GetById1(int id);
    TimeLimitForCredit GetDefault();
    int Insert(LineCredit entity, bool withInternalTrans = true);
    int Insert1(TimeLimitForCredit entity, bool withInternalTrans = true);
    void Rollback();
    bool SetDefault(TimeLimitForCredit newDefault);
    bool Update(LineCredit entity, bool withInternalTrans = true);
}

public class LineCreditsInLiteDbService : ILineCreditsInLiteDbService
{
    readonly LiteDatabase db;
    readonly ILiteCollection<LineCredit> collection;
    readonly ILiteCollection<TimeLimitForCredit> collection1, collection2;

    public LineCreditsInLiteDbService()
    {
        var cnx = new ConnectionString()
        {
            Filename = FileHelper.GetFileDbPath("LineCredits")
        };

        db = new(cnx);
        collection = db.GetCollection<LineCredit>(nameof(LineCredit));
        collection1 = db.GetCollection<TimeLimitForCredit>(nameof(TimeLimitForCredit));
        collection2 = db.GetCollection<TimeLimitForCredit>("Default" + nameof(TimeLimitForCredit));
    }

    public void BeginTrans() => db.BeginTrans();

    public void Commit() => db.Commit();

    public void Rollback() => db.Rollback();

    public void Dispose() => db.Dispose();


    public bool Exist => collection.Count() > 0;

    public bool ExistById(int id) => collection.FindById(id) is not null;

    public LineCredit GetById(int id) => collection.FindById(id);

    public int Insert(LineCredit entity, bool withInternalTrans = true)
    {
        if (withInternalTrans)
        {
            db.BeginTrans();
            try
            {
                var result = collection.Insert(entity).AsInt32;
                db.Commit();
                return result;
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
        else
        {
            return collection.Insert(entity).AsInt32;
        }
    }

    public bool Update(LineCredit entity, bool withInternalTrans = true)
    {
        if (withInternalTrans)
        {
            db.BeginTrans();
            try
            {
                var result = collection.Update(entity);
                db.Commit();
                return result;
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
        else
        {
            return collection.Update(entity);
        }
    }

    public IEnumerable<LineCredit> GetAll() => collection.FindAll();

    public bool Delete(int id, bool withInternalTrans = true)
    {
        if (withInternalTrans)
        {
            db.BeginTrans();
            try
            {
                var result = collection.Delete(id);
                db.Commit();
                return result;
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
        else
        {
            return collection.Delete(id);
        }
    }


    public TimeLimitForCredit GetById1(int id) => collection1.FindById(id);

    public int Insert1(TimeLimitForCredit entity, bool withInternalTrans = true)
    {
        if (withInternalTrans)
        {
            db.BeginTrans();
            try
            {
                var result = collection1.Insert(entity).AsInt32;
                db.Commit();
                return result;
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
        else
        {
            return collection1.Insert(entity).AsInt32;
        }
    }

    public IEnumerable<TimeLimitForCredit> GetAll1() => collection1.FindAll();

    public bool Delete1(int id, bool withInternalTrans = true)
    {
        if (withInternalTrans)
        {
            db.BeginTrans();
            try
            {
                var result = collection1.Delete(id);
                db.Commit();
                return result;
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
        else
        {
            return collection1.Delete(id);
        }
    }

    public TimeLimitForCredit GetDefault() => collection2.FindOne(Query.All());

    public bool SetDefault(TimeLimitForCredit newDefault)
    {
        db.BeginTrans();
        try
        {
            collection2.DeleteAll();
            var result = collection2.Insert(newDefault).AsInt32;
            db.Commit();
            return result > 0;
        }
        catch
        {
            db.Rollback();
            return false;
        }
    }
}
