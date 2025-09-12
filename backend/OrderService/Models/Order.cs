namespace OrderService.Models{
    public class Order
    {
        public string OrderId { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public double Amount { get; set; } = 0;

    }
}