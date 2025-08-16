
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Net.NetworkInformation;

namespace PostSQLgreAPI.Model
{
    [Table("users", Schema = "public")]  // Explicitly map to schema and table
    public class Users
    {
        [Key]
        public int id { get; set; }

        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; } // 👈 store only hash
        public string? profile_image_url { get; set; } // still your PublicId
        public DateTime date_created { get; set; }
    }
}



