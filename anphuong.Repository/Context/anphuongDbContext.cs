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
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(300);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                entity.HasMany(e => e.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                      .WithOne(u => u.Customer)
                      .HasForeignKey<Customer>(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

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

                entity.HasMany(e => e.OrderDetails)
                      .WithOne(od => od.Order)
                      .HasForeignKey(od => od.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).IsRequired().HasColumnType("float");
                entity.Property(e => e.SubTotal).IsRequired().HasColumnType("float");
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();
                entity.Property(e => e.isCustomized).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.CustomLongSize).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.CustomWidthSize).HasColumnType("decimal(18,2)").IsRequired(false);
                entity.Property(e => e.CustomHeightSize).HasColumnType("decimal(18,2)").IsRequired(false);

                entity.HasOne(e => e.Variant)
                      .WithMany()
                      .HasForeignKey(e => e.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

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

                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Variant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.VariantImage).HasMaxLength(200);
                entity.Property(e => e.SKU).HasMaxLength(50).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                entity.HasOne(e => e.Color)
                      .WithMany(c => c.Variants)
                      .HasForeignKey(e => e.ColorId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Product)
                 .WithMany(p => p.Variants)
                 .HasForeignKey(e => e.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Material)
                 .WithMany(p => p.Variants)
                 .HasForeignKey(e => e.MaterialId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.HexCode).IsRequired().HasMaxLength(7);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();
            });

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

                entity.HasOne(d => d.Product)
                      .WithOne(p => p.DetailImage)
                      .HasForeignKey<DetailImage>(d => d.ProductId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.QuantityInStock).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                entity.HasOne(e => e.Variant)
                      .WithOne(v => v.Inventory)
                      .HasForeignKey<Inventory>(i => i.VariantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();
            });

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

            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).HasMaxLength(450).IsRequired(false);
                entity.Property(e => e.ReviewerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.RatingValue).IsRequired();
                entity.Property(e => e.Comment).HasMaxLength(1000).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // Quan hệ 1-N với Product
                entity.HasOne(e => e.Product)
                      .WithMany(p => p.Reviews) // Đảm bảo class Product có: public virtual ICollection<ProductReview> Reviews { get; set; }
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict); // Dùng Restrict để đồng bộ với các bảng khác của sếp
            });

            // Cấu hình bảng Cart
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // Quan hệ 1-1 hoặc 1-N với User (Mỗi user có 1 giỏ hàng active)
                entity.HasOne(e => e.User)
                      .WithMany() // Nếu sếp có list Carts trong class User thì gọi ra đây, không thì để trống
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa User thì xóa luôn giỏ hàng của họ
            });

            // Cấu hình bảng CartItem
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.UpdatedAt).IsRequired().HasColumnType("datetime");
                entity.Property(e => e.IsDeleted).IsRequired();

                // Nối với giỏ hàng
                entity.HasOne(e => e.Cart)
                      .WithMany(c => c.CartItems) // Giả định trong class Cart sếp có ICollection<CartItem> CartItems
                      .HasForeignKey(e => e.CartId)
                      .OnDelete(DeleteBehavior.Cascade); // Xóa giỏ hàng thì xóa luôn các món đồ bên trong

                // Nối với Sản phẩm
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Nối với Biến thể (Variant) - Có thể null nếu sản phẩm ko có biến thể
                entity.HasOne(e => e.Variant)
                      .WithMany()
                      .HasForeignKey(e => e.VariantId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false); // Quan trọng: Cho phép null
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId)
                      .IsRequired();

                entity.Property(e => e.Sender)
                      .IsRequired()
                      .HasMaxLength(20); // Chỉ lưu "User" hoặc "Admin" nên 20 ký tự là dư xài

                entity.Property(e => e.Content)
                      .IsRequired()
                      .HasMaxLength(2000); // Giới hạn tin nhắn 2000 ký tự (tránh spam)

                entity.Property(e => e.Timestamp)
                      .IsRequired()
                      .HasColumnType("datetime");

                entity.Property(e => e.IsRead)
                      .IsRequired()
                      .HasDefaultValue(false); // Mặc định tin nhắn mới là chưa đọc
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}