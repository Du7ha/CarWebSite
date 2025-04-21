// Models/Photo.cs
namespace CarWebSite.Models
{
     public class Photo
    {
       public int Id { get; set; }

    
        public string? Url { get; set; }

        public string? Description { get; set; }

        public int? CarId { get; set; }

        // Navigation property
        public virtual Car? Car { get; set; }
    }
}
