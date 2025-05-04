using System.ComponentModel.DataAnnotations;

namespace CarWebSite.Models
{
    public class ReviewFormViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Review content is required")]
        [StringLength(1000, ErrorMessage = "Review content cannot be longer than 1000 characters")]
        public string ReviewContent { get; set; }
    }
}