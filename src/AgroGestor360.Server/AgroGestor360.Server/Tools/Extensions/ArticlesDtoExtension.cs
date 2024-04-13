using AgroGestor360.Server.Models;
using LiteDB;

namespace AgroGestor360.Server.Tools.Extensions;

public static class ArticlesDtoExtension
{
    public static Article ToArticle(this ArticleDTO dto)
    {
        return new Article
        {
            Id = string.IsNullOrEmpty(dto.Id) ? null : new ObjectId(dto.Id),
            Price = dto.Price,
            MerchandiseId = new ObjectId(dto.Merchandise)
        };
    }

    public static ArticleDTO ToArticleDTO(this Article entity)
    {
        return new ArticleDTO
        {
            Id = entity.Id!.ToString(),
            Price = entity.Price
        };
    }
}
