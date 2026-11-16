namespace MagazaIadeOtomasyonu.Models;

public class ReturnItem
{
    public int ReturnItemId { get; set; }
    public int ReturnId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Action { get; set; } = string.Empty;
}
