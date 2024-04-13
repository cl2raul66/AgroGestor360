using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class WarehouseItemDtoExtension
{
    public static DTO2 ToDTO2(this WarehouseItem entity)
    {
        return new DTO2
        {
            Id = entity.Id!.ToString(),
            Merchandise = entity.Merchandise!.ToDTO1(),
            Quantity = entity.Quantity
        };
    }

    public static WarehouseItem FromDTO2(this DTO2 dTO)
    {
        return new WarehouseItem
        {
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            Merchandise = dTO.Merchandise!.FromDTO1(),
            Quantity = dTO.Quantity
        };
    }

    public static DTO2_1 ToDTO2_1(this WarehouseItem entity)
    {
        return new DTO2_1
        {
            Id = entity.Id!.ToString(),
            Name = entity.Merchandise!.Name,
            Category = entity.Merchandise!.Category!.Name,
            Unit = entity.Merchandise!.Packaging!.Unit,
            Value = entity.Merchandise!.Packaging.Value,
            Quantity = entity.Quantity
        };
    }
}
