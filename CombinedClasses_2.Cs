using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductItemId { get; set; }
        public int Qty { get; set; }
        public Cart Cart { get; set; }
        public ProductItem ProductItem { get; set; }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public int? ParentCategoryId { get; set; }
        public string CategoryName { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<Category> SubCategories { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<ProductVariationCategory> ProductVariationCategories { get; set; }
        public ICollection<PromotionCategory> PromotionCategories { get; set; }
    }

    public class Country
    {
        public int Id { get; set; }
        public string CountryName { get; set; }
        public ICollection<ShippingAddress> UserAddresses { get; set; }
    }

    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public int PaymentMethodId { get; set; }
        public int ShippingAddressId { get; set; }
        public int ShippingMethodId { get; set; }
        public decimal OrderTotal { get; set; }
        public int OrderStatusId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public User User { get; set; }
        public ShippingAddress ShippingAddress { get; set; }
        public ShippingMethod ShippingMethod { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public int ProductItemId { get; set; }
        public int OrderId { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public ProductItem ProductItem { get; set; }
        public Order Order { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }

    public class OrderStatus
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public ICollection<Order> Orders { get; set; }
    }

    public class Payment
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public ICollection<PaymentMethod> UserPaymentMethods { get; set; }
    }

    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string UserId { get; set; }
        public int PaymentTypeId { get; set; }
        public string Provider { get; set; }
        public string AccountNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsDefault { get; set; }
        public User User { get; set; }
        public Payment PaymentType { get; set; }
        public ICollection<Order> orders { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public ICollection<ProductItem> ProductItems { get; set; }
    }

    public class ProductConfiguration
    {
        public int ProductConfigurationId { get; set; }
        public int ProductItemId { get; set; }
        public int VariationOptionId { get; set; }
        public ProductItem ProductItem { get; set; }
        public VariationOption VariationOption { get; set; }
    }

    public class ProductItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public int QtyInStock { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ProductImage { get; set; }
        public string SellerId { get; set; }
        public Product Product { get; set; }
        public Seller Seller { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }
        public ICollection<ProductConfiguration> ProductConfigurations { get; set; }
        public ICollection<WishListItems> WishlistItems { get; set; }
    }

    public class ProductVariationCategory
    {
        public int CategoryId { get; set; }
        public int VariationId { get; set; }
        public Category Category { get; set; }
        public Variation Variation { get; set; }
    }

    public class Promotion
    {
        public int PromotionId { get; set; }
        public string Description { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public ICollection<PromotionCategory> PromotionCategories { get; set; }
    }

    public class PromotionCategory
    {
        public int PromotionId { get; set; }
        public int CategoryId { get; set; }
        public Promotion Promotion { get; set; }
        public Category Category { get; set; }
    }

    public class Review
    {
        [Key]
        public int UserReviewId { get; set; }
        public string UserId { get; set; }
        public int RatingValue { get; set; }
        public string Comment { get; set; }
        public int OrderLineId { get; set; }
        public User User { get; set; }
        public OrderLine OrderLine { get; set; }
    }

    public class Seller
    {
        public string SellerId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool EmailConfirmed { get; set; }
        public ICollection<ProductItem> ProductItems { get; set; }
    }

    public class ShippingAddress
    {
        [Key]
        public int UserAddressId { get; set; }
        public string UserId { get; set; }
        public int CountryId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public bool IsDefault { get; set; }
        public User User { get; set; }
        public Country Country { get; set; }
        public ICollection<Order> orders { get; set; }
    }

    public class ShippingMethod
    {
        public int ShippingMethodId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public ICollection<Order> Orders { get; set; }
    }

    public class User : IdentityUser
    {
        public DateTime RegistrationDate { get; set; }
        public Cart cart { get; set; }
        public int CartId { get; set; }
        public ICollection<ShippingAddress> Addresses { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
        public ICollection<WishListItems> WishlistItems { get; set; }
    }

    public class Variation
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public ICollection<VariationOption> VariationOptions { get; set; }
        public ICollection<ProductVariationCategory> productVariationCategories { get; set; }
    }

    public class VariationOption
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int VariationId { get; set; }
        public Variation Variation { get; set; }
        public ICollection<ProductConfiguration> ProductConfigurations { get; set; }
    }

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
