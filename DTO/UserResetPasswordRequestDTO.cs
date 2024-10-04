﻿using System.Text.Json.Serialization;

namespace E_Commerce.DTO
{
    public class UserResetPasswordRequestDTO
    {
        public string Email { get; set; }

        [JsonIgnore]
        public string ClientUri { get; set; } = @"https://localhost:7104/User/EmailConfirmation";
    }
}
