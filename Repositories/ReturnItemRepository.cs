using Microsoft.Data.SqlClient;
using MagazaIadeOtomasyonu.Data;
using MagazaIadeOtomasyonu.Models;

namespace MagazaIadeOtomasyonu.Repositories;

public class ReturnItemRepository
{
    public void AddItems(int returnId, IEnumerable<ReturnItem> items, SqlConnection connection, SqlTransaction transaction)
    {
        using var command = new SqlCommand(
            @"INSERT INTO ReturnItems (ReturnId, ProductName, Barcode, Quantity, UnitPrice, Action)
              VALUES (@ReturnId, @ProductName, @Barcode, @Quantity, @UnitPrice, @Action);",
            connection,
            transaction);

        command.Parameters.Add("@ReturnId", System.Data.SqlDbType.Int);
        command.Parameters.Add("@ProductName", System.Data.SqlDbType.NVarChar, 150);
        command.Parameters.Add("@Barcode", System.Data.SqlDbType.NVarChar, 50);
        command.Parameters.Add("@Quantity", System.Data.SqlDbType.Int);
        var unitPriceParameter = command.Parameters.Add("@UnitPrice", System.Data.SqlDbType.Decimal);
        unitPriceParameter.Precision = 18;
        unitPriceParameter.Scale = 2;
        command.Parameters.Add("@Action", System.Data.SqlDbType.NVarChar, 10);

        foreach (var item in items)
        {
            command.Parameters["@ReturnId"].Value = returnId;
            command.Parameters["@ProductName"].Value = item.ProductName;
            command.Parameters["@Barcode"].Value = item.Barcode;
            command.Parameters["@Quantity"].Value = item.Quantity;
            command.Parameters["@UnitPrice"].Value = item.UnitPrice;
            command.Parameters["@Action"].Value = item.Action;
            command.ExecuteNonQuery();
        }
    }

    public void DeleteByReturnId(int returnId, SqlConnection connection, SqlTransaction transaction)
    {
        using var command = new SqlCommand(
            "DELETE FROM ReturnItems WHERE ReturnId = @ReturnId;",
            connection,
            transaction);

        command.Parameters.AddWithValue("@ReturnId", returnId);
        command.ExecuteNonQuery();
    }
}
