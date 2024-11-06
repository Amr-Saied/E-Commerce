using E_Commerce.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ECommerceDbContext : IdentityDbContext<User>
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options) { }



    public DbSet<Admin> Admins { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ShippingAddress> ShippingAddresses { get; set; }
    public DbSet<Variance> Variances { get; set; }
    public DbSet<Value> Values { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Admin>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Admin>()
            .Property(a => a.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Product>()
            .Property(p => p.Id)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<User>()
             .HasIndex(u => u.Email)
             .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

        modelBuilder.Entity<Seller>()
            .HasIndex(s => s.UserName)
            .IsUnique();

        modelBuilder.Entity<Seller>()
            .HasIndex(s => s.Email)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .Property(c => c.Id)
            .ValueGeneratedOnAdd();



        // IdentityUser -> Order (One-to-Many)
        modelBuilder.Entity<Order>()
            .HasOne<User>(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId);

        // IdentityUser -> ShippingAddress (One-to-Many)
        modelBuilder.Entity<ShippingAddress>()
            .HasOne<User>(sa => sa.User)
            .WithMany()
            .HasForeignKey(sa => sa.UserId);

        // IdentityUser -> Review (One-to-Many)
        modelBuilder.Entity<Review>()
            .HasOne<User>(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        // Product -> Category (Many-to-One)
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);

        // Product -> Review (One-to-Many)
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId);

        // CartItem -> Product (Many-to-One)
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(p => p.CartItems)
            .HasForeignKey(ci => ci.ProductId);

        // Order -> OrderItem (One-to-Many)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        // OrderItem -> Product (Many-to-One)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId);

        // Order -> Payment (One-to-One)
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithOne(o => o.Payment)
            .HasForeignKey<Payment>(p => p.OrderId);

        // Define the one-to-many relationship between Seller and Product
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Seller)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Cascade);


        // Define the many-to-many relationship between Product and Variance
        modelBuilder.Entity<ProductVariance>()
            .HasKey(pv => new { pv.ProductId, pv.VarianceId });

        modelBuilder.Entity<ProductVariance>()
            .HasOne(pv => pv.product)
            .WithMany(p => p.productVariances)
            .HasForeignKey(pv => pv.ProductId);

        modelBuilder.Entity<ProductVariance>()
            .HasOne(pv => pv.variance)
            .WithMany(v => v.ProductVariances)
            .HasForeignKey(pv => pv.VarianceId);

        // Define the many-to-many relationship between Variance and Value
        modelBuilder.Entity<VarianceValue>()
            .HasKey(vv => new { vv.VarianceId, vv.ValueId });

        modelBuilder.Entity<VarianceValue>()
            .HasOne(vv => vv.variance)
            .WithMany(v => v.VarianceValues)
            .HasForeignKey(vv => vv.VarianceId);

        modelBuilder.Entity<VarianceValue>()
            .HasOne(vv => vv.Value)
            .WithMany(v => v.VarianceValues)
            .HasForeignKey(vv => vv.ValueId);

    }
}
