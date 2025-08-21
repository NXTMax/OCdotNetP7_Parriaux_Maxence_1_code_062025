using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P7CreateRestApi.Models
{
    public class User: IdentityUser
    {
        [Required]
        [StringLength(125)]
        public string FullName { get; set; } = string.Empty;
    }
}