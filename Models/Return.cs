namespace MagazaIadeOtomasyonu.Models;

public class Return
{
    public int ReturnId { get; set; }
    public int CustomerId { get; set; }
    public int CreatedByUserId { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public string Note { get; set; } = string.Empty;
}
