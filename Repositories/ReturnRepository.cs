using Microsoft.Data.SqlClient;
using MagazaIadeOtomasyonu.Data;
using MagazaIadeOtomasyonu.Models;

namespace MagazaIadeOtomasyonu.Repositories;

public class ReturnRepository
{
    private readonly ReturnItemRepository _itemRepository = new();

    public void CreateReturn(Return header, List<ReturnItem> items)
    {
        using var connection = DbHelper.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            using var command = new SqlCommand(
                @"INSERT INTO Returns (CustomerId, CreatedByUserId, RequestType, Status, Reason, RequestDate, Note)
                  VALUES (@CustomerId, @CreatedByUserId, @RequestType, @Status, @Reason, @RequestDate, @Note);
                  SELECT CAST(SCOPE_IDENTITY() AS int);",
                connection,
                transaction);

            command.Parameters.AddWithValue("@CustomerId", header.CustomerId);
            command.Parameters.AddWithValue("@CreatedByUserId", header.CreatedByUserId);
            command.Parameters.AddWithValue("@RequestType", header.RequestType);
            command.Parameters.AddWithValue("@Status", string.IsNullOrWhiteSpace(header.Status) ? "Pending" : header.Status);
            command.Parameters.AddWithValue("@Reason", header.Reason);
            command.Parameters.AddWithValue("@RequestDate", header.RequestDate);
            command.Parameters.AddWithValue("@Note", header.Note);

            var returnId = (int)command.ExecuteScalar();
            _itemRepository.AddItems(returnId, items, connection, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public List<Return> GetReturns()
    {
        var returns = new List<Return>();

        using var connection = DbHelper.CreateConnection();
        using var command = new SqlCommand(
            @"SELECT ReturnId, CustomerId, CreatedByUserId, RequestType, Status, Reason, RequestDate, Note
              FROM Returns
              ORDER BY ReturnId DESC;",
            connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            returns.Add(new Return
            {
                ReturnId = reader.GetInt32(0),
                CustomerId = reader.GetInt32(1),
                CreatedByUserId = reader.GetInt32(2),
                RequestType = reader.GetString(3),
                Status = reader.GetString(4),
                Reason = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                RequestDate = reader.GetDateTime(6),
                Note = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
            });
        }

        return returns;
    }

    public void UpdateStatus(int returnId, string status)
    {
        using var connection = DbHelper.CreateConnection();
        using var command = new SqlCommand(
            "UPDATE Returns SET Status = @Status WHERE ReturnId = @ReturnId;",
            connection);

        command.Parameters.AddWithValue("@Status", status);
        command.Parameters.AddWithValue("@ReturnId", returnId);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void DeleteReturn(int returnId)
    {
        using var connection = DbHelper.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            _itemRepository.DeleteByReturnId(returnId, connection, transaction);

            using var command = new SqlCommand(
                "DELETE FROM Returns WHERE ReturnId = @ReturnId;",
                connection,
                transaction);
            command.Parameters.AddWithValue("@ReturnId", returnId);
            command.ExecuteNonQuery();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
