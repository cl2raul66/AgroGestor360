using AgroGestor360Client.Models;

namespace AgroGestor360App.Models;

public class ProductItem
{
    public double ProductItemQuantity { get; set; }
    public double PriceWhitDiscount { get; set; }
    public DTO4? Product { get; set; }
    public ProductOffering? ProductOffer { get; set; }
    public DiscountForCustomer? CustomerDiscountClass { get; set; }
}
