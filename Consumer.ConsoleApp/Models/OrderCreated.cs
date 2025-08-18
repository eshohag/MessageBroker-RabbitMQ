namespace Consumer.ConsoleApp.Models
{
    public class OrderCreated
    {
        public Guid OrderId { get; init; }
        public string CustomerName { get; init; }
        public decimal TotalAmount { get; init; }
    }
}
