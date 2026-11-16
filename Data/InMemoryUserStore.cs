using MagazaIadeOtomasyonu.Models;

namespace MagazaIadeOtomasyonu.Data;

public static class InMemoryUserStore
{
    public static readonly IReadOnlyList<User> Users = new List<User>
    {
        new User
        {
            UserId = 1,
            Username = "admin",
            Password = "admin123",
            DisplayName = "System Administrator"
        }
    };
}
