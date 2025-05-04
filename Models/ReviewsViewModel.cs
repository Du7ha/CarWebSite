namespace CarWebSite.Models
{
    public class ReviewsViewModel
    {
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public ReviewFormViewModel ReviewForm { get; set; } = new ReviewFormViewModel();
    }
}