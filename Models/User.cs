using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace CarWebSite.Models
{
    public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int UserId { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }  // Changed back to Password

    [Required]
    [EnumDataType(typeof(UserRole))]
    public string Role { get; set; } = UserRole.User.ToString();

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [Required]
    public bool IsSeller { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<Car>? CarsForSale { get; set; }
    public virtual ICollection<Order>? OrdersAsBuyer { get; set; }
    public virtual ICollection<Order>? OrdersAsSeller { get; set; }
    public virtual ICollection<SavedCar>? SavedCars { get; set; }

    public User() { }

    public User(string userName, string email, string password, string phoneNumber, 
               string? location = null, bool isSeller = false, UserRole role = UserRole.User)
    {
        UserName = userName;
        Email = email;
        Password = password;  
        PhoneNumber = phoneNumber;
        Location = location;
        IsSeller = isSeller;
        Role = role.ToString();
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsAdmin() => string.Equals(Role, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase);

    public bool HasValidEmail() => !string.IsNullOrEmpty(Email) && Email.Contains("@");

    public void ToggleSellerStatus() => IsSeller = !IsSeller;

    public bool IsStrongPassword() => !string.IsNullOrEmpty(Password) && 
                                    Password.Length >= 8 &&
                                    Password.Any(char.IsUpper) &&
                                    Password.Any(char.IsLower) &&
                                    Password.Any(char.IsDigit);

    public bool IsValidPhoneNumber() => !string.IsNullOrEmpty(PhoneNumber) &&
                                      PhoneNumber.Length >= 10 &&
                                      PhoneNumber.All(char.IsDigit);
}

public enum UserRole
{
    Admin,
    User,
    Moderator
}
}
