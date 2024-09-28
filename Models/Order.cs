namespace E_Commerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }  // Pending, Shipped, Delivered, etc.

        // Foreign key to User
        public string UserId { get; set; }
        public User User { get; set; }

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; }
        public Payment Payment { get; set; }
    }
}
