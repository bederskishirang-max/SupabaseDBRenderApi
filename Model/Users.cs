
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace PostSQLgreAPI.Model
{
    [Table("users", Schema = "public")]  // Explicitly map to schema and table
    public class Users
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(255)]
        public string username { get; set; }

        
        [MaxLength(255)]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        [Column("date_created")] // Matches your DB column name
        public DateTime date_created { get; set; } = DateTime.UtcNow; // Default value
    }
}



