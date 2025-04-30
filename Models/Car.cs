using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace CarWebSite.Models
{
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CarId { get; set; }

        [Required, StringLength(50)]
        public string Brand { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [Required, Range(1900, 2100)]
        public int Year { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Mileage { get; set; }

        [Required, Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [StringLength(30)]
        public string? Color { get; set; }

        [Required]
        public string BodyType { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? ImagePath { get; set; }

        [Required]
        public bool IsSold { get; set; } = false;

        [Required]
        public int CategoryId { get; set; }

        [Required, ForeignKey("Seller")]
        public string? SellerId { get; set; } = "0";  // FK to Client

        [DataType(DataType.DateTime)]
        public DateTime? ListingDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Client? Seller { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<SavedCar>? SavedByUsers { get; set; }
        public virtual ICollection<Photo>? Photos { get; set; }

        public string GetMainImageUrl() =>
            string.IsNullOrEmpty(ImagePath) ? "/images/default-car-image.jpg" : ImagePath;

        public bool IsValidYear() => Year >= 0;
        public bool IsValidPrice() => Price >= 0;
        public bool IsValidMileage() => Mileage >= 0;
        public bool IsValidNumeric(string s) => int.TryParse(s, out _);
    }

    public enum BodyType
    {
        Sedan,
        SUV,
        Truck,
        Coupe,
        Hatchback,
        Convertible,
        Van,
        Wagon,
        Electric,
        Hybrid,
        Other
    }
}
