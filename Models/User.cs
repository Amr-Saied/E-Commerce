namespace E_Commerce.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }  // Admin, Customer, etc.
        public string JwtToken { get; set; }
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        public ICollection<Order> Orders { get; set; }
        public ICollection<ShippingAddress> ShippingAddresses { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
