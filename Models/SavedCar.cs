using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
    public class SavedCar
    {
        [ForeignKey("Client")]
        public string ClientId { get; set; }

        [ForeignKey("Car")]
        public int CarId { get; set; }

        public DateTime SavedDate { get; set; } = DateTime.UtcNow;

        public virtual Client Client { get; set; }
        public virtual Car Car { get; set; }
    }

}
