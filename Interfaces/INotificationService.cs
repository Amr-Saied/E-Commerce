namespace E_Commerce.Interfaces
{
    public interface INotificationService
    {
        Task NotifyUserWishListAsync(int productItemId);

    }
}
