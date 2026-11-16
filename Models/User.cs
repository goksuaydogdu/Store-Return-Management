namespace MagazaIadeOtomasyonu.Models;

public class User
{
    public int UserId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
}
