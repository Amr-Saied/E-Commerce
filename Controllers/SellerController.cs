using E_Commerce.DTO;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace E_Commerce.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class SellerController : ControllerBase
    {

        private readonly  ECommerceDbContext _Context;
        private readonly TokenService _TokenService;
        private readonly IEmailSender _EmailSender;
        private JwtOptions _jwtOptions;

        public SellerController(ECommerceDbContext Context, TokenService tokenService, IEmailSender emailSender, JwtOptions jwtOptions1)
        {
            _Context  = Context;
            _TokenService = tokenService;
            _EmailSender = emailSender;
            _jwtOptions = jwtOptions1;
        }

        [AllowAnonymous]
        [HttpPost("Add-Seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = _Context.Sellers.FirstOrDefault(s => s.UserName == register.Username || s.Email == register.Email);

            if (userExists != null)
            {
                return BadRequest("Username Or Email already exists.");
            }


            var seller = new Seller
            {
                SellerId = GenerateSecureSellerId(),
                UserName = register.Username,
                Email = register.Email,
                RegistrationDate = DateTime.Now,

            };

            var passwordHasher = new PasswordHasher<Seller>();
            seller.PasswordHash = passwordHasher.HashPassword(seller, register.Password);

            _Context.Sellers.Add(seller);

            var token = _TokenService.GenerateToken(seller);
            var param = new Dictionary<string, string>
            {
                { "token" , token },
                { "email" , seller.Email}
            };

            var callBack = QueryHelpers.AddQueryString(register.ClientUri, param);

            var emailSubject = "Email Confirmation Token";
            var emailContent = $"Please confirm your account by clicking this link: <a href='{callBack}'>link</a>";

            await _EmailSender.SendEmailAsync(seller.Email, emailSubject, emailContent);

            if (_Context.SaveChanges() >= 1)
            {
                return Ok("Registeration Successfuly!");
            }

            return BadRequest("Error occurred during registration.");
        }



        [AllowAnonymous]
        [HttpPost("a3f5d1e2c4b6a7d8e9f0b1c2d3e4f5a6b7c8d9e0f1a2b3c4d5e6f7a8b9c0d1e2")]
        public async Task<IActionResult> SignInSeller([FromBody] LoginDTO login)
        {
            if (string.IsNullOrEmpty(login.EmailOrUserName) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Email/Username and password are required.");
            }

            // Find seller by username or email
            var seller = await _Context.Sellers
                .SingleOrDefaultAsync(s => s.UserName == login.EmailOrUserName || s.Email == login.EmailOrUserName);

            if (seller == null)
            {
                return Unauthorized("Invalid username/email or password.");
            }

            if (!seller.EmailConfirmed)
            {
                return Unauthorized("Email Not Confirmed!");
            }

            // Verify password
            var passwordHasher = new PasswordHasher<Seller>();
            var result = passwordHasher.VerifyHashedPassword(seller, seller.PasswordHash, login.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized("Invalid username/email or password.");
            }

            // Generate token
            var token = TokenGenerator(seller, "Seller");

            return Ok(new
            {
                message = "Sign-in successful!",
                token = token
            });
        }

        [AllowAnonymous]
        [HttpPut("Seller-ConFirm-Email")]
        public IActionResult ConfirmEmail(string token, string email)
        {
            var seller = _Context.Sellers.SingleOrDefault(s => s.Email == email);
            if (seller == null)
            {
                return BadRequest("Invalid seller.");
            }

            var isValidToken = _TokenService.ValidateToken(token, seller);
            if (isValidToken)
            {
                // Mark the email as confirmed
                seller.EmailConfirmed = true;
                _Context.SaveChanges();
                return Ok("Email confirmed!");
            }

            return BadRequest("Invalid or expired token.");
        }

        private string GenerateSecureSellerId()
        {
            string prefix = "Seller-";
            string secureId = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            return prefix + secureId; // Creates a base64-encoded ID with a specific prefix
        }

        private string TokenGenerator(Seller seller, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, seller.SellerId.ToString()),
                    new(ClaimTypes.Name, seller.UserName),
                    new(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTime)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken;
        }
    }
}
