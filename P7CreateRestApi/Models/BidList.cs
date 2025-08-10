using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P7CreateRestApi.Models
{
    public class BidList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BidListId { get; set; }

        [Required]
        [StringLength(30)]
        public string Account { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string BidType { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        public decimal? BidQuantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? AskQuantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Bid { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Ask { get; set; }

        [StringLength(125)]
        public string? Benchmark { get; set; }

        public DateTime? BidListDate { get; set; }

        [StringLength(125)]
        public string? Commentary { get; set; }

        [StringLength(125)]
        public string? Security { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        [StringLength(125)]
        public string? Trader { get; set; }

        [StringLength(125)]
        public string? Book { get; set; }

        [StringLength(125)]
        public string? CreationName { get; set; }

        public DateTime? CreationDate { get; set; }

        [StringLength(125)]
        public string? RevisionName { get; set; }

        public DateTime? RevisionDate { get; set; }

        [StringLength(125)]
        public string? DealName { get; set; }        [StringLength(125)]
        public string? DealType { get; set; }

        [StringLength(125)]
        public string? SourceListId { get; set; }

        [StringLength(10)]
        public string? Side { get; set; }
    }
}