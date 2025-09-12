using Microsoft.Data.SqlClient;
using Dapper;
using OrderService.Models;

namespace SqlHelper
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertOrder(Order order)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            string sql = "";

            connection.Execute(sql, order);
        }
    }
}
