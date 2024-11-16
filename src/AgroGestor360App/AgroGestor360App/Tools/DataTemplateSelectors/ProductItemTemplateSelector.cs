using AgroGestor360App.Models;

namespace AgroGestor360App.Tools.DataTemplateSelectors;

public class ProductItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? NormalPriceTemplate { get; set; }
    public DataTemplate? CustomerDiscountTemplate { get; set; }
    public DataTemplate? ProductOfferTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var productItem = (ProductItem)item;

        if (productItem.CustomerDiscountClass is not null)
        {
            return CustomerDiscountTemplate!;
        }

        if (productItem.ProductOffer is not null)
        {
            return ProductOfferTemplate!;
        }

        return NormalPriceTemplate!;
    }
}
