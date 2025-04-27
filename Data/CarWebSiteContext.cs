using CarWebSite.Models;
using Microsoft.EntityFrameworkCore;

namespace CarWebSite.Data
{
    public class CarWebSiteContext : DbContext
    {
        public CarWebSiteContext(DbContextOptions<CarWebSiteContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SavedCar> SavedCars { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Category> Category { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure decimal precision for monetary fields
            modelBuilder.Entity<Car>()
                .Property(c => c.Price)
                .HasPrecision(18, 2); // 18 digits total, 2 decimal places

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Configure SavedCar -> User relationship
            modelBuilder.Entity<SavedCar>()
                .HasKey(sc => new { sc.UserId, sc.CarId });

            // Configure relationships for SavedCar
            modelBuilder.Entity<SavedCar>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.SavedCars)
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedCar>()
                .HasOne(sc => sc.Car)
                .WithMany(c => c.SavedByUsers)
                .HasForeignKey(sc => sc.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Buyer relationship (User -> Orders)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany(u => u.OrdersAsBuyer)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Seller relationship (User -> Orders)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Seller)
                .WithMany(u => u.OrdersAsSeller)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Car relationship (Order -> Car)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Car)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CarId);

            // Configure Payment -> Order relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User -> Car relationship
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Seller)
                .WithMany(u => u.CarsForSale)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Photo -> Car relationship
            modelBuilder.Entity<Photo>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Car)
                .WithMany(c => c.Photos)
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Category
            modelBuilder.Entity<Category>()
                .HasKey(c => c.CategoryId);

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Category>()
                .Property(c => c.Description)
                .HasMaxLength(500);

            // Car - Category relationship
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Cars)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}