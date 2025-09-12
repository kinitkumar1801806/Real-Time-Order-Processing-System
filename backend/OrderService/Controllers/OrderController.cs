using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using OrderService.Models;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[Controller]/")]
    public class OrderController : ControllerBase
    {
        private readonly string kafkaTopic = "orders";
        private readonly string bootstrapServers = "kafka:9092";
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var message = System.Text.Json.JsonSerializer.Serialize(order);
            try
            {
                await producer.ProduceAsync(kafkaTopic, new Message<Null, string> { Value = message });
                return Ok(new { Message = "Order placed successfully!" });
            }
            catch (ProduceException<Null,string> ex)
            {
                return StatusCode(500,new {Error=$"Unexpected error: {ex.Message}"});
            }
        }
        [HttpGet]
        public IActionResult GetCustomers()
        {
          return Ok(new { Message = "Customers fetched successfully" });
        }
    }
}