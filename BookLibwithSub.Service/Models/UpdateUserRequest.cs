using System.ComponentModel.DataAnnotations;

namespace BookLibwithSub.Service.Models
{
    public class UpdateUserRequest
    {
        // All fields optional; only provided ones will be updated.
        [StringLength(100)] public string? Username { get; set; }
        [StringLength(100)] public string? FullName { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [StringLength(30)] public string? PhoneNumber { get; set; }

        // If provided, we will re-hash it and replace the current password.
        [StringLength(100)] public string? Password { get; set; }
    }
}
