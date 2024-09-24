namespace E_Commerce.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string PaymentMethod { get; set; }  // Credit Card, PayPal, etc.
        public string PaymentStatus { get; set; }  // Completed, Failed, etc.
        public DateTime TransactionDate { get; set; }

        // Foreign key to Order
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
