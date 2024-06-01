using AgroGestor360.Server.Models;
using LiteDB;
using UnitsNet;

namespace AgroGestor360.Server.Tools.Extensions;

public static class ArticleItemForWarehouseDTOExtension
{
    public static DTO2 ToDTO2(this ArticleItemForWarehouse entity)
    {
        return new DTO2
        {
            Quantity = entity.Quantity,
            Reserved = entity.Reserved,
            MerchandiseName = entity.MerchandiseName,
            Packaging = entity.Packaging,
            MerchandiseId = entity.MerchandiseId?.ToString(),
        };
    }

    public static ArticleItemForWarehouse FromDTO2(this DTO2 dTO)
    {
        return new ArticleItemForWarehouse
        {
            Quantity = dTO.Quantity,
            Reserved = dTO.Reserved,
            MerchandiseName = dTO.MerchandiseName,
            Packaging = dTO.Packaging,
            MerchandiseId = string.IsNullOrEmpty(dTO.MerchandiseId) ? null : new ObjectId(dTO.MerchandiseId),
        };
    }
}
