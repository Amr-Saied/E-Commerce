namespace E_Commerce.Models
{
    public class WishListItems
    {
        public int Id { get; set; }

        public string UserId { get; set; }  
        public User User { get; set; }

        public int ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; }

        public bool IsNotified { get; set; } = false;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
    }
}
