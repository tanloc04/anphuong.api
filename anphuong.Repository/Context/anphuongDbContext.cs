using anphuong.Core.Domains.Entities;
using Microsoft.EntityFrameworkCore;

namespace anphuong.Repository.Context
{
    public class anphuongDbContext : DbContext
    {
        public anphuongDbContext() { }
        public anphuongDbContext(DbContextOptions<anphuongDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Behavior> Behaviors { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<DetailImage> DetailImages { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Variant> Variants { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(30);
                entity.Property(e => e.PasswordHash).HasMaxLength(256);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();
             
            });

            // Configuring Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // One-to-many relationship with Orders
                entity.HasMany(e => e.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One-to-one relationship with User
                entity.HasOne(c => c.User)
                      .WithOne() // User type does not declare a Customer navigation in provided signatures
                      .HasForeignKey<Customer>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            // Configuring Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PaymentMethod).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.ShippingDate).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.TotalPrice).IsRequired().HasColumnType("float");
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // One-to-many relationship with OrderDetails
                entity.HasMany(e => e.OrderDetails)
                      .WithOne(od => od.Order)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuring OrderDetail entity
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("float");
                entity.Property(e => e.SubTotal).IsRequired().HasColumnType("float");
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // Many-to-one relationship with Variant
                entity.HasOne(e => e.Variant)
                      .WithMany()
                      .HasForeignKey(e => e.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuring Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).IsRequired().HasColumnType("float");
                entity.Property(e => e.Discount).IsRequired().HasColumnType("float");
                entity.Property(e => e.Description).HasMaxLength(600);
                entity.Property(e => e.LongSize).IsRequired();
                entity.Property(e => e.WidthSize).IsRequired();
                entity.Property(e => e.HeightSize).IsRequired();
                entity.Property(e => e.isCustomize).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();           

                // Many-to-one relationship with Category
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict).IsRequired(false);

            });

            // Configuring Variant entity
            modelBuilder.Entity<Variant>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.VariantImage).HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // Many-to-one relationship with Color
                entity.HasOne(e => e.Color)
                      .WithMany(c => c.Variants)
                      .HasForeignKey(e => e.ColorId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Many-to-one relationship with Product
                entity.HasOne(e => e.Product)
                 .WithMany(p => p.Variants)
                 .HasForeignKey(e => e.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Many-to-one relationship with Material
                entity.HasOne(e => e.Material)
                 .WithMany(p => p.Variants)
                 .HasForeignKey(e => e.MaterialId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuring Color entity
            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HexCode).IsRequired().HasMaxLength(7);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();
            });

            // Configuring DetailImage entity
            modelBuilder.Entity<DetailImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Image1).HasMaxLength(255);
                entity.Property(e => e.Image2).HasMaxLength(255);
                entity.Property(e => e.Image3).HasMaxLength(255);
                entity.Property(e => e.Image4).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // One-to-one relationship with Product             
                entity.HasOne(d => d.Product)
                      .WithOne()
                      .HasForeignKey<DetailImage>(d => d.ProductId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
            });

            // Configuring Inventory entity
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuantityInStock).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // One-to-one relationship with Variant
                // FIX:
                // - Use the existing Inventory.Variant navigation
                // - Do not reference Variant.Inventory (it doesn't exist in provided Variant signature)
                // - Use VariantId as the FK on Inventory (not ProductId)
                entity.HasOne(e => e.Variant)
                      .WithOne() // Variant currently does not declare an Inventory navigation property
                      .HasForeignKey<Inventory>(i => i.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuring Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();
            });        

            // Configuring Behavior entity
            modelBuilder.Entity<Behavior>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ActionType).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Count).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                entity.HasOne(e => e.Customer)
                .WithMany(b => b.Behaviors)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                .WithMany(b => b.Behaviors)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
//Add-Migration InitMigration -Context anphuongDbContext -Project anphuong.Repository -StartupProject anphuong.api -OutputDir Context/Migrations