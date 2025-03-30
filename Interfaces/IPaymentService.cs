namespace E_Commerce.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(string email, string paymentMethodId, string userId, string name);
        Task<bool> CreateStripeCustomerAsync(string name, string email, string systemId);
        Task<bool> HandleWebhookAsync(string json, string signatureHeader);

    }
}