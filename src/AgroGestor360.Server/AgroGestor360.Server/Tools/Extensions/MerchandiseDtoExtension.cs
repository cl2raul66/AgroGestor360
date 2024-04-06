using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class MerchandiseDtoExtension
{
    public static MerchandiseCategory ToMerchandiseCategory(this MerchandiseCategoryDTO dto)
    {
        return new MerchandiseCategory
        {
            Id = string.IsNullOrEmpty(dto.Id) ? null : new ObjectId(dto.Id),
            Name = dto.Name
        };
    }

    public static MerchandiseCategoryDTO ToMerchandiseCategoryDTO(this MerchandiseCategory bank)
    {
        return new MerchandiseCategoryDTO
        {
            Id = bank.Id!.ToString(),
            Name = bank.Name
        };
    }
}
