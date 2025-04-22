// Models/Photo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
    public class Photo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        public string? Url { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [ForeignKey("Car")]
        public int? CarId { get; set; }

        public virtual Car? Car { get; set; }
    }
}
