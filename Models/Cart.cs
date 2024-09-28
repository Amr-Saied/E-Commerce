namespace E_Commerce.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // Foreign key to User
        public string UserId { get; set; }
        public User User { get; set; }

        // Navigation property
        public ICollection<CartItem> CartItems { get; set; }
    }
}
