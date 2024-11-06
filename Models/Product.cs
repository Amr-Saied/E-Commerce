using E_Commerce.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }

 
    public int CategoryId { get; set; }
    public Category Category { get; set; }

   
    public string SellerId { get; set; }
    public Seller Seller { get; set; }

  
    public ICollection<CartItem> CartItems { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<Review> Reviews { get; set; }

    public ICollection<ProductVariance> productVariances { get; set; }
}
