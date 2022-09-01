using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Daurecard.Models
{
    public class Subscribe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Licence-key")]
        public string LicenceKey { get; set; }

        [Required]
        [Display(Name = "ProductId")]
        public int ProductId { get; set; }  
    }
}
