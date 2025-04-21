using CarWebSite.Models;

namespace CarWebSite.ViewModels
{
    public class ProfileViewModel
    {
        public User ?User { get; set; }
        public IEnumerable<Order> ?Orders { get; set; }
        public IEnumerable<SavedCar> ?SavedCars { get; set; }
    }
}