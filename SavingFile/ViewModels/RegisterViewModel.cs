using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SavingFile.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Choose a password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "This value should be the same")]
        [DataType(DataType.Password)]
        [Display(Name = "Password again")]
        public string PasswordConfirm { get; set; }
    }
}
