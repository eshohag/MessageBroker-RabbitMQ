namespace Producer.WebAPI.Models
{
    public class OrderCreated
    {
        public Guid OrderId { get; init; }=Guid.NewGuid();
        public string CustomerName { get; init; } = "Shohag";
        public decimal TotalAmount { get; init; } = 1000;
    }
}
