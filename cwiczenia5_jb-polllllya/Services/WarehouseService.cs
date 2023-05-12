using System.Data.SqlClient;
using Zadanie5.DTOs;

namespace Zadanie5.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IConfiguration _configuration;

        public WarehouseService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Tuple<int, string> AddProduct(ProductDTO product)
        {
            var tuple = new Tuple<int, string>(200, "Ok");

            var connectionString = _configuration.GetConnectionString("Database");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        // Sprawdzamy czy produkt o podanym id istnieje
                        command.CommandText = "SELECT COUNT(*) FROM master.dbo.Product WHERE IdProduct = @idProduct";
                        command.Parameters.AddWithValue("@idProduct", product.IdProduct);

                        var result = (int)command.ExecuteScalar();

                        if (result == 0)
                        {
                            tuple = new Tuple<int, string>(404, "Product with given id does not exist.");
                            return tuple;
                        }

                        // Sprawdzamy czy hurtownia o podanym id istnieje
                        command.CommandText =
                            "SELECT COUNT(*) FROM master.dbo.Warehouse WHERE IdWarehouse = @idWarehouse";
                        command.Parameters.AddWithValue("@idWarehouse", product.IdWarehouse);

                        result = (int)command.ExecuteScalar();

                        if (result == 0)
                        {
                            tuple = new Tuple<int, string>(404, "Warehouse with given id does not exist.");
                            return tuple;
                        }

                        // Upewniamy się, że wartość Amount jest większa od 0
                        if (product.Amount <= 0)
                        {
                            tuple = new Tuple<int, string>(400, "Amount value should be greater than 0.");
                            return tuple;
                        }

                        // Sprawdzamy czy istnieje zlecenie zakupu produktu
                        command.CommandText =
                            "SELECT COUNT(*) FROM master.dbo.[Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
                        command.Parameters.AddWithValue("@Amount", product.Amount);
                        command.Parameters.AddWithValue("@CreatedAt", product.CreatedAt);

                        result = (int)command.ExecuteScalar();

                        if (result == 0)
                        {
                            tuple = new Tuple<int, string>(404, "There is no such order.");
                            return tuple;
                        }

                        // Sprawdzamy czy zlecenie nie zostało już zrealizowane
                        command.CommandText =
                            "SELECT COUNT(*) FROM master.dbo.Product_Warehouse WHERE IdOrder = (SELECT TOP 1 IdOrder FROM master.dbo.[Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt ORDER BY CreatedAt DESC)";

                        result = (int)command.ExecuteScalar();

                        if (result > 0)
                        {
                            tuple = new Tuple<int, string>(400, "The order has already been completed.");
                            return tuple;
                        }

                        // Aktualizujemy kolumnę FullfilledAt w tabeli [Order]
                        command.CommandText =
                            "UPDATE master.dbo.[Order] SET FulfilledAt = @fulfilledAt WHERE IdOrder = (SELECT TOP 1 IdOrder FROM master.dbo.[Order] WHERE IdProduct = @idProduct AND Amount = @Amount AND CreatedAt < @createdAt ORDER BY CreatedAt DESC)";
                        command.Parameters.AddWithValue("@fulfilledAt", DateTime.Now);

                        command.ExecuteNonQuery();


                        // Wstawiamy rekord do tabeli Product_Warehouse
                        command.CommandText =
                            "INSERT INTO master.dbo.Product_Warehouse (IdWarehouse,IdProduct, IdOrder, Amount, Price, CreatedAt) " +
                            "VALUES (@idWarehouse,@idProduct, (SELECT TOP 1 IdOrder FROM master.dbo.[Order] WHERE IdProduct = @idProduct AND Amount = @amount AND CreatedAt < @createdAt ORDER BY CreatedAt DESC), " +
                            "@amount, (SELECT Price * @Amount FROM master.dbo.Product WHERE idProduct = @idProduct), @createdAt)";
                        command.ExecuteNonQuery();


                        // Zwracamy wartość klucza głównego wygenerowanego dla rekordu wstawionego do tabeli Product_Warehouse
                        command.CommandText =
                            "SELECT IdProductWarehouse FROM master.dbo.Product_Warehouse WHERE IdProduct = @idProduct AND IdWarehouse = @idWarehouse ";

                        var kay = (int)command.ExecuteScalar();

                        command.ExecuteNonQuery();

                        tuple = new Tuple<int, string>(200,
                            "The value of the primary key generated for the record inserted into the Product Warehouse table - " + kay);
                    }

                    // Zatwierdzamy transakcję
                    transaction.Commit();

                    return tuple;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}