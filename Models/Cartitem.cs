namespace E_Commerce.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // Foreign keys
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }
}
