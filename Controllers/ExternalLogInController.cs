using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using E_Commerce.DTO;
using E_Commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using E_Commerce.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http; 
using System.Net.Http.Json;
using Google.Apis.Auth;
using Newtonsoft.Json.Linq;


namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalLoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly JwtOptions _jwtOptions;

        public ExternalLoginController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IEmailSender emailSender,
            JwtOptions jwtOptions1)
            
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _jwtOptions = jwtOptions1;
        }



        [HttpGet]
        [Route("GoogleLogin")]
        public IActionResult GoogleLogin()
        {
            var provider = "Google";

            var redirectUrl = "/api/ExternalLogin/signin-google";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }



        [HttpGet]
        [Route("signin-google")]
        public async Task<IActionResult> GoogleResponse(string accessToken)
        {


            var userInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo";

            using (var httpClient = new HttpClient())
            {
                // Set the Authorization header with the access token
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                // Send the request to the UserInfo API
                var response = await httpClient.GetAsync(userInfoEndpoint);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Failed to retrieve user info from Google.");
                }

                // Parse the response
                var content = await response.Content.ReadAsStringAsync();
                var userInfo = JObject.Parse(content);

                // Extract user information
                string email = userInfo["email"]?.ToString();
                string name = userInfo["name"]?.ToString();
                string givenName = userInfo["given_name"]?.ToString();
                string familyName = userInfo["family_name"]?.ToString();
                string pictureUrl = userInfo["picture"]?.ToString();

                // Return user information or handle it as needed
                return Ok(new
                {
                    Name = name,
                    Email = email,
                    GivenName = givenName,
                    FamilyName = familyName,
                    PictureUrl = pictureUrl
                });
            }

            // Call Google's userinfo API with the access token


            //    // At this point, you have the userInfo object populated with user data from Google
            //    var email = userInfo.Email;

            //    // Continue with your existing logic
            //    var result = await _signInManager.ExternalLoginSignInAsync("Google", userInfo.Id, isPersistent: false, bypassTwoFactor: true);
            //    if (result.Succeeded)
            //    {
            //        var ValidUser = await _userManager.FindByLoginAsync("Google", userInfo.Id);
            //        var token = TokenGenerator(ValidUser, "User");
            //        return Ok(new { token = token, message = "Google sign-in successful" });
            //    }

            //    if (result.IsLockedOut)
            //    {
            //        return Unauthorized("User is locked out.");
            //    }

            //    // If the user does not have an account, create one.
            //    var user = await _userManager.FindByEmailAsync(email);
            //    if (user == null)
            //    {
            //        // Create a new user without password
            //        user = new User
            //        {
            //            UserName = email,
            //            Email = email,
            //            RegistrationDate = DateTime.UtcNow
            //        };

            //        var identityResult = await _userManager.CreateAsync(user);
            //        if (!identityResult.Succeeded)
            //        {
            //            return BadRequest("Error creating new user.");
            //        }

            //        await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", userInfo.Id, "Google"));

            //        // Send email confirmation if necessary
            //        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //        var param = new Dictionary<string, string>
            //{
            //    { "token", token },
            //    { "email", user.Email }
            //};

            //        var callBack = QueryHelpers.AddQueryString("https://localhost:7104/User/EmailConfirmation", param);
            //        var emailSubject = "Confirm your email";
            //        var emailContent = $"Please confirm your account by clicking this link: <a href='{callBack}'>link</a>";
            //        await _emailSender.SendEmailAsync(user.Email, emailSubject, emailContent);
            //    }

            //    // Sign the user in after account creation or login
            //    await _signInManager.SignInAsync(user, isPersistent: false);

            //    // Generate JWT token
            //    var jwtToken = TokenGenerator(user, "User");
            //    return Ok(new { token = jwtToken, message = "Google sign-in successful!" });
        }



        //[HttpGet]
        //[Route("signin-google")]
        //public async Task<IActionResult> GoogleResponse(string accessToken)
        //{
        //    var info = await _signInManager.GetExternalLoginInfoAsync();
        //    if (info == null)
        //    {
        //        return BadRequest("Error loading external login information.");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login.
        //    var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        //    if (result.Succeeded)
        //    {
        //        var ValidUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
        //        var token = TokenGenerator(ValidUser, "User");
        //        return Ok(new { token = token, message = "Google sign-in successful" });
        //    }

        //    if (result.IsLockedOut)
        //    {
        //        return Unauthorized("User is locked out.");
        //    }

        //    // If the user does not have an account, create one.
        //    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //    {
        //        // Create a new user without password
        //        user = new User
        //        {
        //            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
        //            Email = email,
        //            RegistrationDate = DateTime.UtcNow
        //        };

        //        var identityResult = await _userManager.CreateAsync(user);
        //        if (!identityResult.Succeeded)
        //        {
        //            return BadRequest("Error creating new user.");
        //        }

        //        await _userManager.AddLoginAsync(user, info);

        //        // Send email confirmation if necessary
        //        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //        var param = new Dictionary<string, string> { 
        //            { "token", token }, 
        //            { "email", user.Email } 
        //        };

        //        var callBack = QueryHelpers.AddQueryString("https://localhost:7104/User/EmailConfirmation", param);
        //        var emailSubject = "Confirm your email";
        //        var emailContent = $"Please confirm your account by clicking this link: <a href='{callBack}'>link</a>";
        //        await _emailSender.SendEmailAsync(user.Email, emailSubject, emailContent);
        //    }

        //    // Sign the user in after account creation or login
        //    await _signInManager.SignInAsync(user, isPersistent: false);

        //    // Generate JWT token
        //    var jwtToken = TokenGenerator(user, "User");
        //    return Ok(new { token = jwtToken, message = "Google sign-in successful!" });
        //}

        private string TokenGenerator(User user, string role)
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

