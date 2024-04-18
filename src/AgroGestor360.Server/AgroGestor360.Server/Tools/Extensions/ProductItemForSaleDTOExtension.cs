using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class ProductItemForSaleDTOExtension
{
    public static ProductItemForSale FromDTO4(this DTO4 dTO)
    {
        return new ProductItemForSale
        {
            Name = dTO.Name,
            Quantity = dTO.Quantity,
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            Article = dTO.Article?.FromDTO3(),
            Offering = dTO.Offering
        };
    }

    public static DTO4 ToDTO4(this ProductItemForSale entity)
    {
        return new DTO4
        {
            Name = entity.Name,
            Quantity = entity.Quantity,
            Id = entity.Id?.ToString(),
            Article = entity.Article?.ToDTO3(),
            Offering = entity.Offering
        };
    }

    public static DTO4_1 ToDTO4_1(this ProductItemForSale entity)
    {
        return new DTO4_1
        {
            Name= entity.Name,
            Quantity = entity.Quantity,
            Id = entity.Id?.ToString(),
            Category = entity.Article?.Merchandise?.Category?.Name,
            Value = entity.Article?.Merchandise?.Packaging?.Value ?? 0,
            Unit = entity.Article?.Merchandise?.Packaging?.Unit,
            SalePrice = (entity.Quantity * entity.Article?.Price) ?? 0,
        };
    }

    public static ProductItemForSale FromDTO4_2(this DTO4_2 dTO)
    {
        return new ProductItemForSale
        {
            Quantity = dTO.Quantity,
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id)
        };
    }

    public static ProductItemForSale FromDTO4_3(this DTO4_3 dTO)
    {
        return new ProductItemForSale
        {
            Offering = dTO.Offering,
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id)
        };
    }

    public static DTO4_3 ToDTO4_3(this ProductItemForSale entity)
    {
        return new DTO4_3
        {
            Offering = entity.Offering,
            Id = entity.Id?.ToString(),
        };
    }
}
