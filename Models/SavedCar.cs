using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
public class SavedCar
{
    [Key, Column(Order = 0)]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Key, Column(Order = 1)]
    [ForeignKey("Car")]
    public int CarId { get; set; }

    public DateTime SavedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Car? Car { get; set; }
}
}
