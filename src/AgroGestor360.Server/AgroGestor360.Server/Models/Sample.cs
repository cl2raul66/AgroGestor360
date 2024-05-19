using System.Collections.ObjectModel;

namespace AgroGestor360.Server.Models;

public class PgAddEditSaleViewModelData
{
    public DTO8_4? CurrentOrder { get; set; }
    public int ProductsPending { get; set; }
    public string? TextInfo { get; set; }
    public DateTime Date { get; set; }
    public string? NoFEL { get; set; }
    public bool HasNoFEL { get; set; }
    public double Stock { get; set; }
    public List<DTO6>? Sellers { get; set; }
    public DTO6? SelectedSeller { get; set; }
    public List<DTO5_1>? Customers { get; set; }
    public DTO5_1? SelectedCustomer { get; set; }
    public List<DTO4>? Products { get; set; }
    public DTO4? SelectedProduct { get; set; }
    public string? Quantity { get; set; }
    public ObservableCollection<string>? PaymentsTypes { get; set; }
    public string? SelectedPaymentType { get; set; }
    public bool OnCredit { get; set; }
    public ObservableCollection<DTO9>? ProductItems { get; set; }
    public DTO9? SelectedProductItem { get; set; }
    public bool IsNormalPrice { get; set; }
    public bool IsCustomerDiscount { get; set; }
    public bool IsProductOffer { get; set; }
    public ObservableCollection<ProductOffering>? Offers { get; set; }
    public ProductOffering? SelectedOffer { get; set; }
    public double Total { get; set; }
}
