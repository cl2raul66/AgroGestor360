using AgroGestor360.Server.Models;

namespace AgroGestor360.Server.Tools.Extensions;

public static class QuotationDTOExtension
{
    public static DTO7 ToDTO7(this Quotation entity)
    {
        DTO7 dTO = new()
        {
            Code = entity.Code.ToString().Trim('-').ToUpper(),
            QuotationDate = entity.QuotationDate,
            SellerName = entity.Seller?.Contact?.FormattedName,
            SellerId = entity.Seller?.Id?.ToString(),
            CustomerName = string.IsNullOrEmpty(entity.Customer?.Contact?.Organization?.Name)
                    ? entity.Customer?.Contact?.FormattedName
                    : entity.Customer?.Contact?.Organization?.Name,
            TotalAmount = 0,
        };
        double totalAmount = 0;
        foreach (var pi in entity.ProductItems!)
        {
            double itemPrice = pi.Product!.ArticlePrice;
            if (pi.HasCustomerDiscount)
            {
                double discount = entity.Customer!.Discount!.Value;
                itemPrice -= itemPrice * (discount / 100);
            }

            double itemQuantity = pi.Quantity;
            if (pi.OfferId > 0)
            {
                itemQuantity = pi.Product.Offering![pi.OfferId - 1].Quantity;
            }
            totalAmount += itemQuantity * itemPrice;
        }
        dTO.TotalAmount = totalAmount;
        return dTO;
    }
}
