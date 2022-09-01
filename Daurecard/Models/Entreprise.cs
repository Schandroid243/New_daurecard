using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Daurecard.Models
{
    public class Entreprise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required]
        [Display(Name = "Adresse")]
        public string Adresse { get; set; }

        [Required]
        [Display(Name = "Activité")]
        public string SecteurActivite { get; set; }

        [Required]
        [Display(Name = "Téléphone")]
        public string Tel { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Display(Name = "Logo")]
        public string Logo { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile LogoFile { get; set; }
    }
}
