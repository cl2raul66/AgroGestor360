using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class WarehouseItemDtoExtension
{
    public static WarehouseItem ToWarehouseItem(this WarehouseItemSendDTO dTO)
    {
        return new WarehouseItem
        {
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            MerchandiseId = new ObjectId(dTO.MerchandiseId),
            Quantity = dTO.Quantity
        };
    }

    public static WarehouseItemGetDTO ToWarehouseItemDTO(this WarehouseItem entity)
    {
        return new WarehouseItemGetDTO
        {
            Id = entity.Id!.ToString(),
            Quantity = entity.Quantity
        };
    }
}
