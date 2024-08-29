using AgroGestor360.Client.Tools;

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
    public ProductTable[]? ProductItems { get; set; }
    #endregion
    public class ProductTable
    {
        public string? ProductName { get; set; }
        public double ProductQuantity { get; set; }
        public double ArticlePrice { get; set; }
    }
}
#endregion

#region Sale
public class SaleReport()
{
    #region ORGANIZATION
    public string? OrganizationName { get; set; }
    public string? OrganizationAddress { get; set; }
    public string? OrganizationPhone { get; set; }
    public string? OrganizationEmail { get; set; }
    #endregion
    #region STATIC INFORMATION
    public DateTime IssueDate { get; set; }
    public string? SaleState { get; set; }
    public string? OrderBy { get; set; }
    #endregion
    #region SALE ITEMS
    public SaleTable[]? SaleItems { get; set; }
    #endregion
    public class SaleTable
    {
        public string? Code { get; set; }
        public DateTime SaleEntryDate { get; set; }
        public DateTime? SaleDate { get; set; }
        public SaleStatus SaleStatus { get; set; }
        public string? Seller { get; set; }
        public string? Customer { get; set; }
        public double TotalToPay { get; set; }
        public double TotalPaid { get; set; }
    }
}
#endregion
