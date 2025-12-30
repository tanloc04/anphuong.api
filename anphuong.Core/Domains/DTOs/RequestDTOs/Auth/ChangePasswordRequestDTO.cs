using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace anphuong.Core.Domains.DTOs.RequestDTOs.Auth
{
    public class ChangePasswordRequestDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Old Password is required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
