using System.ComponentModel.DataAnnotations;

namespace BookLibwithSub.Service.Models
{
    public class UpdateUserRequest
    {

        [StringLength(100)] public string? Username { get; set; }
        [StringLength(100)] public string? FullName { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [StringLength(30)] public string? PhoneNumber { get; set; }

        [StringLength(100)] public string? Password { get; set; }
    }
}
