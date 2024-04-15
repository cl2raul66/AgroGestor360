using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class ArticleItemForSaleDTOExtension
{
    public static ArticleItemForSale FromDTO3(this DTO3 dTO)
    {
        return new ArticleItemForSale
        {
            Price = dTO.Price,
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            Merchandise = dTO.Merchandise?.FromDTO1()
        };
    }

    public static DTO3 ToDTO3(this ArticleItemForSale entity)
    {
        return new DTO3
        {
            Price = entity.Price,
            Id = entity.Id!.ToString(),
            Merchandise = entity.Merchandise!.ToDTO1()
        };
    }

    public static DTO3_1 ToDTO3_1(this ArticleItemForSale entity)
    {
        return new DTO3_1
        {
            Price = entity.Price,
            Id = entity.Id!.ToString(),
            Name = entity.Merchandise!.Name,
            Category = entity.Merchandise!.Category!.Name,
            Unit = entity.Merchandise!.Packaging!.Unit,
            Value = entity.Merchandise!.Packaging.Value
        };
    }
}
