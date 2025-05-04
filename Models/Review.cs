using CarWebSite.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Review
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Review content is required")]
    [StringLength(1000, ErrorMessage = "Review content cannot be longer than 1000 characters")]
    public string ReviewContent { get; set; }

    // Remove [Required] because this will be set in the controller
    public string UserId { get; set; }

    public string Author { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    // Ensure that the ClientId property exists
    public string ClientId { get; set; }

    [ForeignKey("ClientId")]
    public virtual Client Client { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}