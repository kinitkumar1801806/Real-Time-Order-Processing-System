using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using System.Text.Json;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderConsumerService
    {
        private readonly string bootstrapServers = "kafka:9092";
        private readonly string kafkaTopic = "orders";
        private readonly string groupId = "order-consumer-group";

        public async Task StartConsuming(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(kafkaTopic);

            Console.WriteLine("Kafka consumer started. Listening for messages...");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        var order = JsonSerializer.Deserialize<Order>(consumeResult.Message.Value);
                        Console.WriteLine($"Received Order Id: {order.Id}, Customer: {order.CustomerName}, Total: {order.TotalAmount}");

                        // TODO: Handle order processing logic here
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
