using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace CarWebSite.Models
{
    public class CarModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int carId { get; set; }

        [Required]
        public string brand { get; set; }

        
        public string model { get; set; }

        [Required]
        public int year { get; set; }

        [Required]
        public int mileAge { get; set; }

        [Required]
        public int price { get; set; }

        public string color { get; set; }
        public string bodyType { get; set; }
        public string description { get; set; }
        public bool isSold { get; set; }

        [Required]

        public int userId { get; set; }// the user id that is selling this car 

        public CarModel() { }

        public CarModel(string brand, string model, int year, int mileAge, int price, string color, string bodyType, string description, bool isSold, int userId)
        {
            this.brand = brand;
            this.model = model;
            this.year = year;
            this.mileAge = mileAge;
            this.price = price;
            this.color = color;
            this.bodyType = bodyType;
            this.description = description;
            this.isSold = isSold;
            this.userId = userId;
        }

        public bool IsValidYear()// check if the year is not empty and non negative
        {
            return year >= 0; 
        }
        public bool IsValidPrice()// check if the price is not empty and non negative
        {
            return price >= 0; 
        }
        
        public bool IsValidMileage()// check if the mileage is not empty and non negative
        {
            return mileAge >= 0; 
        }
        
        public bool IsValidNumeric(string s )// check if the price is a valid numeric value
        {
            return int.TryParse(s, out int parsedValue);
        }

    }
}
