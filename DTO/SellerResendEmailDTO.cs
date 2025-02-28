using System.Text.Json.Serialization;

namespace E_Commerce.DTO
{
    public class SellerResendEmailDTO
    {
        public string Email { get; set; }

        [JsonIgnore]
        public string ClientUri { get; set; } = @"https://ecommerce.amrkhaled.me/Seller/Seller-ConFirm-Email";
    }
}
