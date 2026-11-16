using Microsoft.Data.SqlClient;

namespace MagazaIadeOtomasyonu.Data;

public static class DbHelper
{
    public static string ConnectionString { get; set; } =
        "Server=YOUR_SERVER\\SQLEXPRESS;Database=MagazaIadeDb;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
