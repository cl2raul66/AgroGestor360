using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class ArticleItemForWarehouseDTOExtension
{
    public static DTO2 ToDTO2(this ArticleItemForWarehouse entity)
    {
        return new DTO2
        {
            Quantity = entity.Quantity,
            Id = entity.Id!.ToString(),
            Merchandise = entity.Merchandise!.ToDTO1()
        };
    }

    public static ArticleItemForWarehouse FromDTO2(this DTO2 dTO)
    {
        return new ArticleItemForWarehouse
        {
            Quantity = dTO.Quantity,
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            Merchandise = dTO.Merchandise!.FromDTO1()
        };
    }

    public static DTO2_1 ToDTO2_1(this ArticleItemForWarehouse entity)
    {
        return new DTO2_1
        {
            Quantity = entity.Quantity,
            Id = entity.Id!.ToString(),
            Name = entity.Merchandise!.Name,
            Category = entity.Merchandise!.Category!.Name,
            Unit = entity.Merchandise!.Packaging!.Unit,
            Value = entity.Merchandise!.Packaging.Value
        };
    }
}
