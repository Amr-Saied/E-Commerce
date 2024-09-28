namespace E_Commerce.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int Rating { get; set; }  // 1-5 stars
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }

        // Foreign keys
        public string UserId { get; set; }
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
