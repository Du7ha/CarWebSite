using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarWebSite.Models
{
    public class Client
    {
        public string Id { get; set; }

        [ForeignKey("User")]
        public string UserID {  get; set; }

        public virtual User User { get; set; }
        [Required]
        public bool IsSeller { get; set; }
        // Navigation properties
        public virtual ICollection<Car>? CarsForSale { get; set; }
        public virtual ICollection<Order>? OrdersAsBuyer { get; set; }
        public virtual ICollection<Order>? OrdersAsSeller { get; set; }
        public virtual ICollection<SavedCar>? SavedCars { get; set; }
        // إضافة هذه الخاصية لتمثيل العلاقة مع الـ Review
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
