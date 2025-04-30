using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int CarId { get; set; }

        [Required]
        public string BuyerId { get; set; }

        [Required]
        public string SellerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        // Navigation
        public virtual Car Car { get; set; }
        public virtual Client Buyer { get; set; }
        public virtual Client Seller { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
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
