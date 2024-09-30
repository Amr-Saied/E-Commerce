using System;
using System.Collections.Generic;

namespace E_Commerce.Models
{
    public class Seller
    {
        public string SellerId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistrationDate { get; set; }

        public bool EmailConfirmed { get; set; }

        public ICollection<Product> Products { get; set; }  // A seller can have multiple products
    }
}
