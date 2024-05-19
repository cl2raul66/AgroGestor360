namespace AgroGestor360.Client.Models;

#region QUOTE
public class CustomerQuoteReport()
{
    #region ORGANIZATION
    public string? OrganizationName { get; set; }
    public string? OrganizationAddress { get; set; }
    public string? OrganizationPhone { get; set; }
    public string? OrganizationEmail { get; set; }
    #endregion
    #region QUOTATION INFORMATION
    public string? QuotationCode { get; set; }
    public DateTime QuotationDate { get; set; }
    public double TotalAmount { get; set; }
    #endregion
    #region SELLER
    public string? SellerName { get; set; }
    #endregion
    #region CUSTOMER
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    #endregion
    #region PRODUCT ITEMS
    public Product[]? ProductItems { get; set; }
    #endregion
    public class Product
    {
        public string? ProductName { get; set; }
        public double ProductQuantity { get; set; }
        public double ArticlePrice { get; set; }
    }
}
#endregion
