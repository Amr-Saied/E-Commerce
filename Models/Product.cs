using E_Commerce.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string ImageUrl { get; set; }

    // Foreign key and navigation property to Category
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    // Foreign key and navigation property to Seller
    public string SellerId { get; set; }
    public Seller Seller { get; set; }

    // Navigation properties
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<Review> Reviews { get; set; }
}
