using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace CarWebSite.Models
{
    public class UserModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int userId { get; set; }// incremented automatically by the database

        [Required]
        public string? userName { get; set; }

        [Required]
        public string? email { get; set; }

        [Required]
        public string? password { get; set; }

        [Required]
        public string? phone { get; set; }

        public string? location { get; set; }

        public bool isSeller { get; set; }//seller or not seller

        public string? role { get; set; }// admin or user

        public DateTime createdAt { get; set; }// the date when the user created his account


        public UserModel() { }

        public UserModel(string userName, string email, string password, string phone, string location, bool isSeller = false, string role = "user")
        {
            this.userName = userName;
            this.email = email;
            this.password = password;
            this.phone = phone;
            this.location = location;
            this.isSeller = isSeller;
            this.role = role;
            this.createdAt = DateTime.UtcNow;
        }


        public bool IsAdmin()// check if the user is admin
        {
            return string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase);
        }

        public bool HasValidEmail()
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        public void ToggleSellerStatus()// Change the user status to seller or not seller
        {
            isSeller = !isSeller;
        }

        public bool IsStrongPassword()// Validate password strength
        {
            return !string.IsNullOrEmpty(password) && password.Length >= 8;
        }

        public bool IsValidPhoneNumber()
        {
            return !string.IsNullOrEmpty(phone) &&
                   phone.Length == 11 &&
                   phone.All(char.IsDigit);
        }

    }
}
