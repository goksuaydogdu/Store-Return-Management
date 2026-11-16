using MagazaIadeOtomasyonu.Models;

namespace MagazaIadeOtomasyonu.Data;

public static class SessionContext
{
    public static bool IsAuthenticated => CurrentUser is not null;
    public static User? CurrentUser { get; private set; }
    public static int UserId => CurrentUser?.UserId ?? 0;

    public static void SignIn(User user)
    {
        CurrentUser = user;
    }

    public static void SignOut()
    {
        CurrentUser = null;
    }
}
