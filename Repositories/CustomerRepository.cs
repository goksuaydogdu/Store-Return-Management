using Microsoft.Data.SqlClient;
using MagazaIadeOtomasyonu.Data;
using MagazaIadeOtomasyonu.Models;

namespace MagazaIadeOtomasyonu.Repositories;

public class CustomerRepository
{
    public List<Customer> GetAll()
    {
        var customers = new List<Customer>();

        using var connection = DbHelper.CreateConnection();
        using var command = new SqlCommand(
            "SELECT CustomerId, FullName, Phone, Email FROM Customers",
            connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            customers.Add(new Customer
            {
                CustomerId = reader.GetInt32(0),
                FullName = reader.GetString(1),
                Phone = reader.GetString(2),
                Email = reader.GetString(3)
            });
        }

        return customers;
    }

    public void Add(Customer customer)
    {
        using var connection = DbHelper.CreateConnection();
        using var command = new SqlCommand(
            @"INSERT INTO Customers (FullName, Phone, Email)
              VALUES (@FullName, @Phone, @Email);",
            connection);

        command.Parameters.AddWithValue("@FullName", customer.FullName);
        command.Parameters.AddWithValue("@Phone", customer.Phone);
        command.Parameters.AddWithValue("@Email", customer.Email);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Update(Customer customer)
    {
        using var connection = DbHelper.CreateConnection();
        using var command = new SqlCommand(
            @"UPDATE Customers
              SET FullName = @FullName,
                  Phone = @Phone,
                  Email = @Email
              WHERE CustomerId = @CustomerId;",
            connection);

        command.Parameters.AddWithValue("@CustomerId", customer.CustomerId);
        command.Parameters.AddWithValue("@FullName", customer.FullName);
        command.Parameters.AddWithValue("@Phone", customer.Phone);
        command.Parameters.AddWithValue("@Email", customer.Email);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int customerId)
    {
        using var connection = DbHelper.CreateConnection();
        using var command = new SqlCommand(
            "DELETE FROM Customers WHERE CustomerId = @CustomerId;",
            connection);

        command.Parameters.AddWithValue("@CustomerId", customerId);

        connection.Open();
        command.ExecuteNonQuery();
    }
}
