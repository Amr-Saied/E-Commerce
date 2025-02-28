﻿using System.Text.Json.Serialization;

namespace E_Commerce.DTO
{
    public class UserResetPasswordRequestDTO
    {
        public string Email { get; set; }

        [JsonIgnore]
        public string ClientUri { get; set; } = @"https://ecommerce.amrkhaled.me/User/EmailConfirmation";
    }
}
