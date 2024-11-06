using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace E_Commerce.Models
{
    public class User : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }

        // Navigation properties
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<ShippingAddress> ShippingAddresses { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
