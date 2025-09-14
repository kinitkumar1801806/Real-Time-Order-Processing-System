using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;
using OrderService.Models;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace OrderService.Services
{
    public class OrderConsumerService : BackgroundService
    {
        private readonly string bootstrapServers = "localhost:9093";
        private readonly string kafkaTopic = "orders";
        private readonly string groupId = "order-consumer-group";
        private readonly string _connectionString;

        public OrderConsumerService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("SqlServer")??"";
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString), 
                    "Connection string 'SqlServer' not found in appsettings.json");
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(kafkaTopic);

            Console.WriteLine("Kafka consumer started. Listening for messages...");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(1000, stoppingToken);
                        var consumeResult = consumer.Consume(stoppingToken);
                        var order = JsonSerializer.Deserialize<Order>(consumeResult.Message.Value);
                        using var connection = new SqlConnection(_connectionString);
                        string sql = @"
                        INSERT INTO Orders (Customer, Amount)
                        VALUES (@Customer, @Amount)";

                        await connection.ExecuteAsync(sql, order);

                        // TODO: Handle order processing logic here
                        Console.WriteLine($"📥 Order consumed: {order?.Customer} - {order?.Amount}");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error consuming message: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Consumer stopping...");
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}
