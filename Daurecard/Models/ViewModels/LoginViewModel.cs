using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Daurecard.Models.ViewModels
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
        }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me ?")]
        public bool RememberMe { get; set; }

    }
}
