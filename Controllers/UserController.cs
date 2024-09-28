﻿using Azure.Core;
using DnsClient;
using E_Commerce.DTO;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;



namespace E_Commerce.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class UserController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtOptions _jwtOptions;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailSender _emailSender;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, JwtOptions jwtoptions, ILogger<UserController> logger, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtOptions = jwtoptions;
            _logger = logger;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpPost("Add-User")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = await _userManager.FindByNameAsync(register.Username) != null ||
                             await _userManager.FindByEmailAsync(register.Email) != null ||
                             _userManager.Users.Any(u => u.PhoneNumber == register.PhoneNumber);

            if (userExists)
            {
                return BadRequest("Username, Email, or Phone Number already exists.");
            }

            var user = new User
            {
                UserName = register.Username,
                Email = register.Email,
                PhoneNumber = register.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                { "token" , token },
                { "email" , user.Email}
            };

            var callBack = QueryHelpers.AddQueryString(register.ClientUri, param);

            var emailSubject = "Email Confirmation Token";
            var emailContent = $"Please confirm your account by clicking this link: <a href='{callBack}'>link</a>";

            await _emailSender.SendEmailAsync(user.Email, emailSubject, emailContent);


            if (result.Succeeded)
            {
                return Ok("Registration successful!");
            }

            return BadRequest("Error occurred during registration.");
        }

        [AllowAnonymous]
        [HttpPost("Signin")]
        public async Task<IActionResult> Signin([FromBody] LoginDTO login)
        {
            if (string.IsNullOrEmpty(login.EmailOrUserName) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Username/Email and password are required.");
            }

            var user = await _userManager.FindByNameAsync(login.EmailOrUserName) ??
                       await _userManager.FindByEmailAsync(login.EmailOrUserName);

            if (user == null)
            {
                return Unauthorized("Invalid username/email or password.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Email Not Confirmed!");

            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("Invalid username/email or password.");
            }

            var token = TokenGenerator(user);

            return Ok(new
            {
                message = "Sign-in successful!",
                token = token
            });
        }

        [AllowAnonymous]
        [HttpGet("EmailConfirmation")]
        public async Task<IActionResult> EmailConfirmation(string token, string email)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Invalid token or email.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Email confirmed successfully! You can now sign in.");
            }

            return BadRequest("Error confirming email.");
        }


        private string TokenGenerator(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey)), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.UserName),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.LifeTime)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            return accessToken;
        }


        [AllowAnonymous]
        [HttpPost("Resend-Email-Confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailDTO resendEmailDTO)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailDTO.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                { "token" , token },
                { "email" , user.Email}
            };

            var callBack = QueryHelpers.AddQueryString(resendEmailDTO.ClientUri, param);

            var emailSubject = "Resend-Email-Token";
            var emailContent = $"Confirm Your Email By Clicking On This Link: <a href='{callBack}'>link</a>";

            await _emailSender.SendEmailAsync(user.Email, emailSubject, emailContent);

            return Ok("ReConfirm Email Was Sent Successfully!");
        }

        [AllowAnonymous]
        [HttpPost("Reset-Password-Request")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestDTO ResetPasswordReq)
        {
            var user = await _userManager.FindByEmailAsync(ResetPasswordReq.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string>
            {
                { "token" , token },
                { "email" , user.Email}
            };

            var callBack = QueryHelpers.AddQueryString(ResetPasswordReq.ClientUri, param);

            var emailSubject = "Password Reset Token";
            var emailContent = $"Reset Your Password by clicking this link: <a href='{callBack}'>link</a>";

            await _emailSender.SendEmailAsync(user.Email, emailSubject, emailContent);

            return Ok("Password Reset Email was sent Successfully!");
            
        }

        [AllowAnonymous]
        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDTO.Token, resetPasswordDTO.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password reset successful!");
            }

            return BadRequest("Error occurred during password reset.");
        }

    }
}
