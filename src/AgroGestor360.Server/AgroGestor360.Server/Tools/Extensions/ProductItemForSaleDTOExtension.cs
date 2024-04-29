using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class ProductItemForSaleDTOExtension
{
    public static DTO4 ToDTO4(this ProductItemForSale entity)
    {
        return new DTO4
        {
            ArticlePrice = entity.ArticlePrice,
            ProductQuantity = entity.ProductQuantity,
            ProductName = entity.ProductName,
            Id = entity.Id?.ToString(),
            MerchandiseId = entity.MerchandiseId?.ToString(),
            Packaging = entity.Packaging,
            HasOffers = entity.Offering is not null && entity.Offering.Length > 0, 
        };
    }

    public static ProductItemForSale FromDTO4_1(this DTO4_1 dTO)
    {
        return new ProductItemForSale
        {
            ArticlePrice = dTO.ArticlePrice,
            ProductQuantity = dTO.ProductQuantity,
            ProductName = dTO.ProductName,
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            MerchandiseId = string.IsNullOrEmpty(dTO.MerchandiseId) ? null : new ObjectId(dTO.MerchandiseId),
            Packaging = dTO.Packaging
        };
    }
}
