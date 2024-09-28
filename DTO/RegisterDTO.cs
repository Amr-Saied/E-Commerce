using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Text.Json.Serialization;

namespace E_Commerce.DTO
{
    public class RegisterDTO
    {
        public string Username { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        [JsonIgnore]
        public string ClientUri { get; set; } = @"https://localhost:7104/User/EmailConfirmation";
    }
}
