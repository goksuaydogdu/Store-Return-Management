using MagazaIadeOtomasyonu.Models;

namespace MagazaIadeOtomasyonu.Repositories;

public interface IUserRepository
{
    User? FindByCredentials(string username, string password);
}
