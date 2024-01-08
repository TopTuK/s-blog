using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Repositary.Entities.User
{
    [Table("UserProfile")]
    public class DbUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string FirstName { get; set; }
        
        public string? LastName { get; set; }

        public bool IsAdmin { get; set; } = false;
    }
}
