﻿using AgroGestor360.Client.Models;

namespace AgroGestor360.App.Models;

public class ProductItem
{
    public double ProductItemQuantity { get; set; }
    public double PriceWhitDiscount { get; set; }
    public DTO4? Product { get; set; }
    public ProductOffering? ProductOffer { get; set; }
    public CustomerDiscountClass? CustomerDiscountClass { get; set; }
}
