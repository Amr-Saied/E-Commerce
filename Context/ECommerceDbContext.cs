using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Models;

namespace E_Commerce.Context
{
    public class ECommerceDbContext : IdentityDbContext<User>
    {
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductConfiguration> ProductConfigurations { get; set; }
        public DbSet<ProductVariationCategory> ProductVariationCategories { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionCategory> PromotionCategories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<ShippingAddress> ShippingAddresses { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Variation> Variations { get; set; }
        public DbSet<VariationOption> VariationOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // Cart and User
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.ShoppingCart)
                .HasForeignKey<Cart>(c => c.UserId);

            // CartItem relationships
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.ProductItem)
                .WithMany(pi => pi.CartItems)
                .HasForeignKey(ci => ci.ProductItemId);

            // Category and Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            // PromotionCategory relationships
            modelBuilder.Entity<PromotionCategory>()
                .HasKey(pc => new { pc.PromotionId, pc.CategoryId });

            modelBuilder.Entity<PromotionCategory>()
                .HasOne(pc => pc.Promotion)
                .WithMany(p => p.PromotionCategories)
                .HasForeignKey(pc => pc.PromotionId);

            modelBuilder.Entity<PromotionCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.PromotionCategories)
                .HasForeignKey(pc => pc.CategoryId);

            // Variance and VarianceValue
            modelBuilder.Entity<VariationOption>()
                .HasKey(vv => new { vv.VariationId, vv.Id });

            modelBuilder.Entity<VariationOption>()
                .HasOne(vv => vv.Variation)
                .WithMany(v => v.VariationOptions)
                .HasForeignKey(vv => vv.VariationId);

            // ShippingAddress and Country
            modelBuilder.Entity<ShippingAddress>()
                .HasOne(sa => sa.Country)
                .WithMany(c => c.UserAddresses)
                .HasForeignKey(sa => sa.CountryId);

            // Order and User
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Order and OrderStatus
            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderStatus)
                .WithMany(os => os.ShopOrders)
                .HasForeignKey(o => o.OrderStatusId);

            // ProductItem and Product
            modelBuilder.Entity<ProductItem>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductItems)
                .HasForeignKey(pi => pi.ProductId);

            // ProductItem and Seller
            modelBuilder.Entity<ProductItem>()
                .HasOne(pi => pi.Seller)
                .WithMany(s => s.ProductItems)
                .HasForeignKey(pi => pi.SellerId);

            // Define many-to-many relationship between Category and Variation using ProductVariationCategory
            modelBuilder.Entity<ProductVariationCategory>()
                .HasKey(pvc => new { pvc.CategoryId, pvc.VariationId });

            modelBuilder.Entity<ProductVariationCategory>()
                .HasOne(pvc => pvc.Category)
                .WithMany(c => c.ProductVariationCategories)
                .HasForeignKey(pvc => pvc.CategoryId);

            modelBuilder.Entity<ProductVariationCategory>()
                .HasOne(pvc => pvc.Variation)
                .WithMany(v => v.productVariationCategories)
                .HasForeignKey(pvc => pvc.VariationId);
        }
    }
}
