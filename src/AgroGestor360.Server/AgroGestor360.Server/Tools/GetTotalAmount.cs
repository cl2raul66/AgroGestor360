using AgroGestor360.Server.Models;

namespace AgroGestor360.Server.Tools;

public static class GetTotalAmount
{
    public static double Get<T>(T entity) where T : SaleBase
    {
        double totalAmount = 0;
        foreach (var p in entity.Products!)
        {
            double itemPrice = p.Product!.ArticlePrice;
            if (p.HasCustomerDiscount)
            {
                double discount = entity.Customer!.Discount!.Discount;
                itemPrice -= itemPrice * (discount / 100);
            }

            double itemQuantity = p.Quantity;
            if (p.OfferId > 0)
            {
                itemQuantity = p.Product.Offering![p.OfferId - 1].Quantity;
            }
            totalAmount += itemQuantity * itemPrice;
        }

        return totalAmount;
    }

}
