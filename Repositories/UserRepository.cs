using MagazaIadeOtomasyonu.Data;
using MagazaIadeOtomasyonu.Models;

namespace MagazaIadeOtomasyonu.Repositories;

public class UserRepository : IUserRepository
{
    public User? FindByCredentials(string username, string password)
    {
        return InMemoryUserStore.Users.FirstOrDefault(user =>
            string.Equals(user.Username, username, StringComparison.OrdinalIgnoreCase) &&
            user.Password == password);
    }
}
