using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P7CreateRestApi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(125)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(125)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(125)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(125)]
        public string Role { get; set; } = string.Empty;
    }
}