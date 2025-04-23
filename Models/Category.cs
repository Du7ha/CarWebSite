// Models/Category.cs
using System.ComponentModel.DataAnnotations;

namespace CarWebSite.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; } // Added primary key

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public virtual ICollection<Car>? Cars { get; set; } // Navigation property  
    }
}