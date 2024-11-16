using AgroGestor360Server.Models;

namespace AgroGestor360Server.Tools;

public static class ProductItemForDocumentToString
{
    public static string GetText(double productQuantity, ProductItemForSale product, bool hasCustomerDiscount, int offerId, Customer customer)
    {
        string texto;

        if (hasCustomerDiscount)
        {
            texto = string.Format("{0} x {1} [{2:F2} {3}] (Descuento) precio: {4:C}",
                productQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                product.ArticlePrice -= product.ArticlePrice * (customer!.Discount!.Discount / 100.00));
        }
        else if (offerId > 0)
        {
            var o = product.Offering![offerId - 1];
            texto = string.Format("{0} x {1} [{2:F2} {3}] (Oferta: {0} + {4} {5} extra) precio: {6:C}",
                o.Quantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                o.BonusAmount,
                o.BonusAmount == 1 ? "unidad" : "unidades",
                product.ArticlePrice);
        }
        else
        {
            texto = string.Format("{0} x {1} [{2:F2} {3}] precio: {4:C}",
                productQuantity,
                product.ProductName,
                product.Packaging!.Value,
                product.Packaging!.Unit,
                product.ArticlePrice);
        }

        return texto;
    }
}
