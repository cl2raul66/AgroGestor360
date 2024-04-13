using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class MerchandiseDtoExtension
{
    public static DTO1 ToDTO1(this MerchandiseItem entity)
    {
        return new DTO1
        {
            Id = entity.Id!.ToString(),
            Name = entity.Name,
            Packaging = entity.Packaging,
            Category = entity.Category,
            Description = entity.Description
        };
    }

    public static MerchandiseItem FromDTO1(this DTO1 dTO)
    {
        return new MerchandiseItem
        {
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            Name = dTO.Name,
            Packaging = dTO.Packaging,
            Category = dTO.Category,
            Description = dTO.Description
        };
    }
}
