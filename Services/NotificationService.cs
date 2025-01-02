using E_Commerce.Context;
using E_Commerce.Interfaces;
using E_Commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ECommerceDbContext _context;
        private readonly IEmailSender _emailSender;
        public NotificationService(ECommerceDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }
        public async Task NotifyUserWishListAsync(int productItemId)
        {
            var productItem = await _context.ProductItems
           .Include(p => p.WishlistItems)
               .ThenInclude(w => w.User) 
           .FirstOrDefaultAsync(p => p.Id == productItemId);
            if (productItem == null)
            {
                throw new ArgumentException($"No ProductItem found with ID {productItemId}");
            }

            foreach (var wishlistItem in productItem.WishlistItems)
            {
                    if (wishlistItem.User == null) continue;
                    var userEmail = wishlistItem.User.Email;
                    var productName = wishlistItem.ProductItem.Product.Name;

                    var subject = $"Good news! Your wishlisted item is now available.";
                    var body = $"Dear {wishlistItem.User.UserName},\n\n" +
                               $"The item '{productName}' you added to your wishlist is now back in stock. " +
                               $"Hurry and grab it before it's gone!\n\n" +
                               $"Best regards,\nEcommerce_team";

                    await _emailSender.SendEmailAsync(userEmail, subject, body);
                    wishlistItem.IsNotified = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
