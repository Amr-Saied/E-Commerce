﻿namespace E_Commerce.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string CountryName { get; set; }

        public ICollection<ShippingAddress> UserAddresses { get; set; }
    }
}
