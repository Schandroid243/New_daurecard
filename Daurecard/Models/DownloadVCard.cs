using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Daurecard.Models
{
    public class DownloadVCard
    {
        public DownloadVCard()
        {
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        public int ClientId { get; set; }
        
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
}
