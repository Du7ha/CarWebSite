using System.ComponentModel.DataAnnotations;

namespace CarWebSite.Models
{
        public class EditVM
        {
            [Required(ErrorMessage = "Username is required.")]
            [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            [Phone(ErrorMessage = "Please enter a valid phone number.")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "City is required.")]
            [StringLength(100, ErrorMessage = "City name cannot be longer than 100 characters.")]
            public string Location { get; set; }

        }


    
}
