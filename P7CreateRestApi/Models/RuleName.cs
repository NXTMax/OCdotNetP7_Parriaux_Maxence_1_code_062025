using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P7CreateRestApi.Models
{
    public class RuleName
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [StringLength(125)]
        public string? Name { get; set; }

        [StringLength(125)]
        public string? Description { get; set; }

        [StringLength(125)]
        public string? Json { get; set; }

        [StringLength(512)]
        public string? Template { get; set; }

        [StringLength(125)]
        public string? SqlStr { get; set; }

        [StringLength(125)]
        public string? SqlPart { get; set; }
    }
}