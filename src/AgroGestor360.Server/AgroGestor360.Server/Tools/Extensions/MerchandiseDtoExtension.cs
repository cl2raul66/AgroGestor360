using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class MerchandiseDtoExtension
{
    public static MerchandiseCategory ToMerchandiseCategory(this MerchandiseCategoryDTO dTO)
    {
        return new MerchandiseCategory
        {
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            Name = dTO.Name
        };
    }

    public static MerchandiseCategoryDTO ToMerchandiseCategoryDTO(this MerchandiseCategory entity)
    {
        return new MerchandiseCategoryDTO
        {
            Id = entity.Id!.ToString(),
            Name = entity.Name
        };
    }

    public static Merchandise ToMerchandise(this MerchandiseDTO dTO)
    {
        return new Merchandise
        {
            Id = string.IsNullOrEmpty(dTO.Id) ? null : new ObjectId(dTO.Id),
            MerchandiseCategoryId = new ObjectId(dTO.MerchandiseCategory),
            Name = dTO.Name,
            Packaging = dTO.Packaging,
            Description = dTO.Description
        };
    }

    public static MerchandiseDTO ToMerchandiseDTO(this Merchandise entity)
    {
        return new MerchandiseDTO
        {
            Id = entity.Id!.ToString(),
            Name = entity.Name,
            Packaging = entity.Packaging,
            Description = entity.Description
        };
    }
}
