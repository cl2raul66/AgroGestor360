using AgroGestor360.Server.Models;
using AgroGestor360.Server.Tools.Enums;

namespace AgroGestor360.Server.Tools.Extensions;

public static class OrdersDTOExtension
{
    public static DTO8 ToDTO8(this Order entity)
    {
        DTO8 dTO = new()
        {
            IsPendingStatus = entity.Status is OrderStatus.Pending,
            Code = entity.Code,
            Date = entity.Date,
            SellerName = entity.Seller?.Contact?.FormattedName,
            SellerId = entity.Seller?.Id?.ToString(),
            CustomerName = string.IsNullOrEmpty(entity.Customer?.Contact?.Organization?.Name)
                    ? entity.Customer?.Contact?.FormattedName
                    : entity.Customer?.Contact?.Organization?.Name,
            CustomerId = entity.Customer?.Id?.ToString(),
            TotalAmount = 0,
        };
        double totalAmount = 0;
        foreach (var pi in entity.Products!)
        {
            double itemPrice = pi.Product!.ArticlePrice;
            if (pi.HasCustomerDiscount)
            {
                double discount = entity.Customer!.Discount!.Discount;
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
