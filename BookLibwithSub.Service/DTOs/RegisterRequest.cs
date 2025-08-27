using System.ComponentModel.DataAnnotations;
using BookLibwithSub.Service.Constants;

namespace BookLibwithSub.Service.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username là bắt buộc.")]
        [RegularExpression(@"^(?![0-9])[a-zA-Z0-9_]{3,20}$",
            ErrorMessage = "Username phải từ 3-20 ký tự, không bắt đầu bằng số, chỉ chứa chữ/số/_")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password là bắt buộc.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,}$",
        //    ErrorMessage = "Password tối thiểu 6 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên là bắt buộc.")]
        [MinLength(3, ErrorMessage = "Họ tên phải ít nhất 3 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^(\+84|0)\d{8,10}$",
            ErrorMessage = "Số điện thoại phải từ 9-11 số, bắt đầu bằng 0 hoặc +84.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public string Role { get; set; } = Roles.User;
    }
}
