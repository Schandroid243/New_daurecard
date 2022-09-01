using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;


namespace Daurecard.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required]
        [Display(Name = "Post-nom")]
        public string PostNom { get; set; }

        [Required]
        [Display(Name = "Téléphone")]
        public string Tel { get; set; }

        [Required]
        [Display(Name = "Facebook")]
        public string Facebook { get; set; }

        [Required]
        [Display(Name = "Twitter")]
        public string Twitter { get; set; }

        [Required]
        [Display(Name = "LinkdeIn")]
        public string LinkdeIn { get; set; }

        public int EntrepriseId { get; set; }

        [ForeignKey("EntrepriseId")]
        public virtual Entreprise Entreprise { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Display(Name = "Photo profile")]
        public string ProfileImage { get; set; }

        [NotMapped]
        [Display(Name = "Sélectionnez une image")]
        public IFormFile ImageFile { get; set; }
    }
}
