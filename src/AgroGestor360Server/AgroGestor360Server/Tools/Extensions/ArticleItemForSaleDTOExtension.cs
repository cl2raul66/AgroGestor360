using AgroGestor360Server.Models;
using LiteDB;

namespace AgroGestor360Server.Tools.Extensions;

public static class ArticleItemForSaleDTOExtension
{
    public static DTO3 ToDTO3(this ArticleItemForSale entity)
    {
        return new DTO3
        {
            Price = entity.Price,
            MerchandiseName = entity.MerchandiseName,
            Packaging = entity.Packaging,
            MerchandiseId = entity.MerchandiseId?.ToString()
        };
    }

    public static ArticleItemForSale FromDTO3(this DTO3 dTO)
    {
        return new ArticleItemForSale
        {
            Price = dTO.Price,
            MerchandiseName = dTO.MerchandiseName,
            Packaging = dTO.Packaging,
            MerchandiseId = string.IsNullOrEmpty(dTO.MerchandiseId) ? null : new ObjectId(dTO.MerchandiseId)
        };
    }
}
