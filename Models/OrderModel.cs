using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int CarId { get; set; }

        public DateTime OrderDate { get; private set; }

        public OrderStatus Status { get; private set; } = OrderStatus.Pending;

        public decimal TotalAmount { get; private set; }

        public string? ShippingAddress { get; set; }

        public string? ContactPhone { get; set; }

        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual UserModel? User { get; set; }

        [ForeignKey("CarId")]
        public virtual CarModel? Car { get; set; }

        public virtual ICollection<Payment>? Payments { get; set; }

        // Default constructor for EF Core
        public Order() { }

        public Order(int userId, int carId, decimal totalAmount, string? shippingAddress = null, string? contactPhone = null, string? notes = null)
        {
            if (userId <= 0)
                throw new ArgumentException("Valid UserId must be provided", nameof(userId));

            if (carId <= 0)
                throw new ArgumentException("Valid CarId must be provided", nameof(carId));

            if (totalAmount <= 0)
                throw new ArgumentException("Total amount must be positive", nameof(totalAmount));

            UserId = userId;
            CarId = carId;
            TotalAmount = totalAmount;
            ShippingAddress = shippingAddress;
            ContactPhone = contactPhone;
            Notes = notes;
            OrderDate = DateTime.UtcNow;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            // Validate status transitions
            if (Status == OrderStatus.Cancelled && newStatus != OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a cancelled order");

            if (Status == OrderStatus.Completed && newStatus != OrderStatus.Completed && newStatus != OrderStatus.Refunded)
                throw new InvalidOperationException("Completed order can only be changed to refunded");

            Status = newStatus;

            // If order is completed, mark the car as sold
            if (newStatus == OrderStatus.Completed && Car != null)
            {
                Car.isSold = true;
            }
        }

        public bool IsValid()
        {
            return UserId > 0 &&
                   CarId > 0 &&
                   TotalAmount > 0 &&
                   OrderDate <= DateTime.UtcNow;
        }

        public bool HasValidContactInfo()
        {
            return !string.IsNullOrEmpty(ShippingAddress) &&
                   !string.IsNullOrEmpty(ContactPhone) &&
                   ContactPhone.Length >= 10 &&
                   ContactPhone.All(char.IsDigit);
        }
    }

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
