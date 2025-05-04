using CarWebSite.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CarWebSite.Data
{
    public class CarWebSiteContext : IdentityDbContext<User>
    {
        public CarWebSiteContext(DbContextOptions<CarWebSiteContext> options) : base(options) { }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SavedCar> SavedCars { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Review> Reviews { get; set; } // Add Reviews DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Car>().Property(c => c.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);

            modelBuilder.Entity<SavedCar>()
                .HasKey(sc => new { sc.ClientId, sc.CarId });

            modelBuilder.Entity<SavedCar>()
                .HasOne(sc => sc.Client)
                .WithMany(c => c.SavedCars)
                .HasForeignKey(sc => sc.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SavedCar>()
                .HasOne(sc => sc.Car)
                .WithMany(c => c.SavedByUsers)
                .HasForeignKey(sc => sc.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)
                .WithMany(c => c.OrdersAsBuyer)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Seller)
                .WithMany(c => c.OrdersAsSeller)
                .HasForeignKey(o => o.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Car)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CarId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Car>()
                .HasOne(c => c.Seller)
                .WithMany(client => client.CarsForSale)
                .HasForeignKey(c => c.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Car)
                .WithMany(c => c.Photos)
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.ReviewContent)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(e => e.Rating)
                      .IsRequired();

                entity.Property(e => e.Date)
                      .IsRequired()
                      .HasDefaultValueSql("GETUTCDATE()");  // Add default value

                entity.Property(e => e.Author)
                      .IsRequired()
                      .HasMaxLength(256);  // Add max length

                entity.Property(e => e.UserId)
                      .IsRequired();  // Ensure it's required in DB

                entity.Property(e => e.ClientId)
                      .IsRequired();  // Ensure it's required in DB

                // Relationship with Client
                entity.HasOne(e => e.Client)
                      .WithMany(c => c.Reviews)
                      .HasForeignKey(e => e.ClientId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Relationship with User
                entity.HasOne(e => e.User)
                      .WithMany()  // If User doesn't have a navigation property for Reviews
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            }); // تعديل حسب احتياجك
        }
    }

}