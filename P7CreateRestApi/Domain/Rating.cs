using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P7CreateRestApi.Domain
{
    public class Rating
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(125)]
        public string? MoodysRating { get; set; }

        [StringLength(125)]
        public string? SandPRating { get; set; }

        [StringLength(125)]
        public string? FitchRating { get; set; }

        public byte? OrderNumber { get; set; }
    }
}