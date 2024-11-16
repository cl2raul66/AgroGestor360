using AgroGestor360Server.Models;
using LiteDB;

namespace AgroGestor360Server.Tools.Extensions;

public static class MerchandiseItemDTOExtension
{
    public static DTO1 ToDTO1(this MerchandiseItem entity)
    {
        return new DTO1
        {
            Name = entity.Name,
            Packaging = entity.Packaging,
            Category = entity.Category,
            Description = entity.Description,
            Id = entity.Id!.ToString()
        };
    }

    public static MerchandiseItem FromDTO1(this DTO1 dTO)
    {
        return new MerchandiseItem
        {
            Name = dTO.Name,
            Packaging = dTO.Packaging,
            Category = dTO.Category,
            Description = dTO.Description,
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id)
        };
    }
}
