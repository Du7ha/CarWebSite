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
            // Configure SavedCar -> User relationship
            modelBuilder.Entity<SavedCar>()
                .HasKey(sc => new { sc.UserId, sc.CarId });

            // Configure relationships for SavedCar
            modelBuilder.Entity<SavedCar>()
                .HasOne(sc => sc.User)
                .WithMany(u => u.SavedCars)  // Assuming a User can have many saved cars
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete for saved cars

            modelBuilder.Entity<SavedCar>()
                .HasOne(sc => sc.Car)
                .WithMany(c => c.SavedByUsers)  // A Car can be saved by many users
                .HasForeignKey(sc => sc.CarId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete for saved cars

            // Configure Buyer relationship (User -> Orders)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Buyer)  // Order has one Buyer (User)
                .WithMany(u => u.OrdersAsBuyer)  // A User can have many Orders as Buyer
                .HasForeignKey(o => o.BuyerId)  // Foreign Key in Order
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete (optional)

            // Configure Seller relationship (User -> Orders)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Seller)  // Order has one Seller (User)
                .WithMany(u => u.OrdersAsSeller)  // A User can have many Orders as Seller
                .HasForeignKey(o => o.SellerId)  // Foreign Key in Order
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete (optional)

            // Configure Car relationship (Order -> Car)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Car)  // Order has one Car
                .WithMany(c => c.Orders)  // A Car can have many Orders
                .HasForeignKey(o => o.CarId);  // Foreign Key in Order

            // Configure Payment -> Order relationship
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)  // Payment belongs to one Order
                .WithMany(o => o.Payments)  // An Order can have many Payments
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete for payments

            // Configure User -> Car relationship (User is the seller of the car)
            modelBuilder.Entity<Car>()
                .HasOne(c => c.Seller)  // Car belongs to one User (Seller)
                .WithMany(u => u.CarsForSale)  // A User can have many Cars for sale
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete for cars


            // Configure Photo -> Car relationship
            modelBuilder.Entity<Photo>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Photo>()
                .HasOne(p => p.Car)
                .WithMany(c => c.Photos) // You'll need to add `public ICollection<Photo> Photos` in Car
                .HasForeignKey(p => p.CarId)
                .OnDelete(DeleteBehavior.SetNull); // If car is deleted, keep photo with null CarId

            // Configure Category
            modelBuilder.Entity<Category>()
                .HasKey(c => c.Name); // Name is primary key

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Category>()
                .Property(c => c.Description)
                .HasMaxLength(500);
        }
    }
}
