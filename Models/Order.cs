using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
    public class Order
    {
        // Primary key for the order
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        // Foreign key referencing the buyer (User)
        [Required]
        [ForeignKey("Buyer")]
        public int BuyerId { get; set; }

        // Foreign key referencing the seller (User)
        [Required]
        [ForeignKey("Seller")]
        public int SellerId { get; set; }

        // Foreign key referencing the car
        [Required]
        [ForeignKey("Car")]
        public int CarId { get; set; }

        // Automatically set to current time on order creation
        [Required]
        public DateTime OrderDate { get; private set; }

        // Enum to track order status (default is Pending)
        [Required]
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        // Total price of the order
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be positive")]
        public decimal TotalAmount { get; private set; }

        // Optional shipping info
        public string? ShippingAddress { get; set; }

        // Optional contact phone
        public string? ContactPhone { get; set; }

        // Optional notes for the order
        public string? Notes { get; set; }

        // Navigation properties for EF Core relationships
        public virtual User? Buyer { get; set; }
        public virtual User? Seller { get; set; }
        public virtual Car? Car { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }

        // Parameterless constructor required by EF Core
        public Order() { }

        // Custom constructor for creating an order
        public Order(int BuyerId, int carId, decimal totalAmount, string? shippingAddress = null, string? contactPhone = null, string? notes = null)
        {
            if (BuyerId <= 0)
                throw new ArgumentException("Valid BuyerId must be provided", nameof(BuyerId));

            if (carId <= 0)
                throw new ArgumentException("Valid CarId must be provided", nameof(carId));

            if (totalAmount <= 0)
                throw new ArgumentException("Total amount must be positive", nameof(totalAmount));

            // ❗ Issue: Shadowing the property name with the parameter name
            // This line doesn't work as intended because it assigns the parameter to itself
            BuyerId = BuyerId;

            // The correct way would be: this.BuyerId = BuyerId;

            CarId = carId;
            TotalAmount = totalAmount;
            ShippingAddress = shippingAddress;
            ContactPhone = contactPhone;
            Notes = notes;
            OrderDate = DateTime.UtcNow;
        }

        // Method to update the status of the order with rules
        public void UpdateStatus(OrderStatus newStatus)
        {
            if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a cancelled order");

            if (Status == OrderStatus.Completed && newStatus != OrderStatus.Completed && newStatus != OrderStatus.Refunded)
                throw new InvalidOperationException("Completed order can only be changed to refunded");

            Status = newStatus;

            // If order is completed, mark the car as sold
            if (newStatus == OrderStatus.Completed && Car != null)
            {
                Car.IsSold = true;
            }
        }

        // Check if the order is logically valid
        public bool IsValid()
        {
            return BuyerId > 0 &&
                   CarId > 0 &&
                   TotalAmount > 0 &&
                   OrderDate <= DateTime.UtcNow;
        }

        // Check if contact info is present and valid
        public bool HasValidContactInfo()
        {
            return !string.IsNullOrEmpty(ShippingAddress) &&
                   !string.IsNullOrEmpty(ContactPhone) &&
                   ContactPhone.Length >= 10 &&
                   ContactPhone.All(char.IsDigit);
        }
    }

    // Enum for order status
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Completed,
        Cancelled,
        Refunded
    }
}
