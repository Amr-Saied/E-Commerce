using E_Commerce.Models;
using Microsoft.AspNetCore.DataProtection;

public class TokenService
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly string _purpose = "EmailConfirmation";

    public TokenService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider;
    }

    public string GenerateToken(Seller seller)
    {
        var protector = _dataProtectionProvider.CreateProtector(_purpose);
        var token = protector.Protect(seller.Email + ":" + DateTime.UtcNow);
        return token;
    }

    public bool ValidateToken(string token, Seller seller)
    {
        var protector = _dataProtectionProvider.CreateProtector(_purpose);
        try
        {
            var unprotectedData = protector.Unprotect(token);
            var data = unprotectedData.Split(':');
            var email = data[0];
            var timestamp = DateTime.Parse(data[1]);

            return email == seller.Email && (DateTime.UtcNow - timestamp).TotalHours < 24;
        }
        catch
        {
            return false;  // Token validation failed
        }
    }
}
