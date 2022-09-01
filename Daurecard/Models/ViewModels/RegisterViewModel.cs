using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Daurecard.Models.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {
        }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email incorrect.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Le {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Le mot de passe et confirmer mot de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
