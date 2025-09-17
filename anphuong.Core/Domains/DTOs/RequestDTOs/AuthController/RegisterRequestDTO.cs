using System.ComponentModel.DataAnnotations;
using static anphuong.Core.Constants.Consts;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.AuthController
{
    public class RegisterRequestDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Display(Name = "Password")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(PASSWORD_MIN_LENGTH, ErrorMessage = "{0} must be at least {1} characters")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "{0} is required")]
        [Compare("Password", ErrorMessage = "Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = null!;

        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Fullname is required")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        public string CustomerAddress { get; set; } = null!;
    }
}
