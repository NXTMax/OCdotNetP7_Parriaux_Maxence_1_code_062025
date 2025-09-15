using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P7CreateRestApi.Models
{
    public class CurvePoint
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        public byte? CurveId { get; set; }

        public DateTime? AsOfDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Term { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Value { get; set; }

        public DateTime? CreationDate { get; set; }
    }
}