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

    [Required]
    [StringLength(50)]
    public string Brand { get; set; }

    [Required]
    [StringLength(50)]
    public string Model { get; set; }

    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }  // 0 if new

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }  // Changed from int to decimal

    [StringLength(30)]
    public string? Color { get; set; }

    [Required]
    [EnumDataType(typeof(BodyType))]
    public string BodyType { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? ImagePath { get; set; }

    [Required]
    public bool IsSold { get; set; } = false;

    [Required]
    [ForeignKey("Seller")]
    public int UserId { get; set; }  // The seller's UserId

    [DataType(DataType.DateTime)]
    public DateTime? ListingDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? Seller { get; set; }
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<SavedCar>? SavedByUsers { get; set; }

    public Car() { }

    public Car(string brand, string model, int year, int mileage, decimal price, 
              string bodyType, int userId, string? color = null, 
              string? description = null, string? imagePath = null)
    {
        Brand = brand;
        Model = model;
        Year = year;
        Mileage = mileage;
        Price = price;
        BodyType = bodyType;
        UserId = userId;
        Color = color;
        Description = description;
        ImagePath = imagePath;
        IsSold = false;
        ListingDate = DateTime.UtcNow;
    }

        public bool IsValidYear()// check if the year is not empty and non negative
        {
            return Year >= 0; 
        }
        public bool IsValidPrice()// check if the price is not empty and non negative
        {
            return Price >= 0; 
        }
        
        public bool IsValidMileage()// check if the mileage is not empty and non negative
        {
            return Mileage >= 0; 
        }
        
        public bool IsValidNumeric(string s )// check if the price is a valid numeric value
        {
            return int.TryParse(s, out int parsedValue);
        }

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
