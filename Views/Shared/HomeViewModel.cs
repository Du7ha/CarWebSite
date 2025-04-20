namespace CarWebSite.Models
{
    public class HomeViewModel
    {
        public List<Photo> Photos { get; set; } = new List<Photo>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}