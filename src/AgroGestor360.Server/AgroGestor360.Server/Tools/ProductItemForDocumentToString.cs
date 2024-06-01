using AgroGestor360.Server.Models;

namespace AgroGestor360.Server.Tools;

public static class ProductItemForDocumentToString
{
    public static string GetText(ProductItemForSale product, bool hasCustomerDiscount, int offerId, Customer customer)
    {
        string texto;

        if (hasCustomerDiscount)
        {
            texto = string.Format("{0,0:F2} {1,-20:F2} {2,0:F2} {3,0} (Descuento) {4,-10:N2}",
                product.ProductQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                product.ArticlePrice -= product.ArticlePrice * (customer!.Discount!.Discount / 100.00));
        }
        else if (offerId > 0)
        {
            var o = product.Offering![offerId - 1];
            texto = string.Format("{0,0:F2}-{1,0} {2,0:F2} {3,0} (Oferta {4,-2} y {5,-2} extra) {6,-10:N2}",
                product.ProductQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                o.BonusAmount,
                o.BonusAmount == 1 ? "unidad" : "unidades",
                product.ArticlePrice);
        }
        else
        {
            texto = string.Format("{0,0:F2} {1,-20:F2} {2,0:F2} {3,0} {4,-10:N2}",
                product.ProductQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                product.ArticlePrice);
        }

        return texto;
    }
}
