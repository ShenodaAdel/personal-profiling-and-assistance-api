using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models
{
    public class ContactUs
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(300)")]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [Column(TypeName = "nvarchar(255)")]
        public string? Email { get; set; }

        [Column(TypeName = "nvarchar(15)")]
        public string? Phone { get; set; }
        
        [Required] 
        public DateTime Date { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Problem { get; set; }

        public int IsRead { get; set; }
    }
}
