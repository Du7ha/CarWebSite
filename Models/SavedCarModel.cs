using System.ComponentModel.DataAnnotations;

namespace CarWebSite.Models
{
    public class SavedCarModel
    {
        [Required]
        public int userId { get; set; } // from User model 
        [Required]
        public int carId { get; set; }// from Car model


    }
}
