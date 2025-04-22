// Models/Category.cs
using System.ComponentModel.DataAnnotations;

namespace CarWebSite.Models
{
    public class Category
    {
        [Key]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}