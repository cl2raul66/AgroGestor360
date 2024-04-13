using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Configurations;
using LiteDB;

namespace AgroGestor360.Server.Sevices;

public interface IArticlesForLitedbService
{
    bool Exist { get; }

    bool Delete(ObjectId id);
    IEnumerable<Article> GetAllEnabled();
    IEnumerable<Article> GetAllDisabled();
    Article GetById(ObjectId id);
    string Insert(Article entity);
    bool Update(Article entity);
}

public class ArticlesForLitedbService : IArticlesForLitedbService
{
    readonly ILiteCollection<Article> collection;

    public ArticlesForLitedbService(ProductsDbConfig dbConfig)
    {
        collection = dbConfig.Bd.GetCollection<Article>();
    }

    public bool Exist => collection.Count() > 0;

    public IEnumerable<Article> GetAllEnabled() => collection.Find(x => !x.Disabled);

    public IEnumerable<Article> GetAllDisabled() => collection.Find(x => x.Disabled);

    public Article GetById(ObjectId id) => collection.FindById(id);

    public string Insert(Article entity) => collection.Insert(entity).AsObjectId.ToString();

    public bool Update(Article entity) => collection.Update(entity);

    public bool Delete(ObjectId id) => collection.Delete(id);
}
